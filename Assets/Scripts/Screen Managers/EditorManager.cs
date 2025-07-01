using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System;
using Entities;
using Controllers;
using Services;
using UseCases;
using Repositories;
using System.Linq;

public class EditorManager : MonoBehaviour
{
    // Screen Elements: Options for adding new Catalogue
    //[SerializeField] private GameObject optionsAddCatalogue;

    // Screen Elements: Question Selection
    [SerializeField] private GameObject questionSelectionScrollView;
    [SerializeField] private TextMeshProUGUI CatalogueLabel;
    [SerializeField] private Toggle isPrivateCheckbox;
    [SerializeField] private GameObject questionButtonPrefab;      // used for dynamically rendering question buttons
    [SerializeField] private Transform buttonContainer;            // 'content' element of scroll view
    [SerializeField] private GameObject emergencyReturnButton;     // used in case selected catalogue could not be loaded for editor
    [SerializeField] private Button deleteTopicButton;

    // Screen Elements: Question Editor
    [SerializeField] private GameObject questionEditor;
    [SerializeField] private Button questionName;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];

    // Screen Elements: Text Input
    [SerializeField] private GameObject textInputView;
    [SerializeField] private TMP_InputField textInputField;
    [SerializeField] private TextMeshProUGUI textInputHeader;

    [SerializeField] private HexagonBackground bg = null;


    // Quiz Area Labels
    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();

    // topic
    [HideInInspector] public static string currentTopicName;

    // catalogue
    private Catalogue currentCatalogue;
    private Question currentQuestion;
    //private CatalogueTable catalogueTable;

    // safety
    private bool invalidStart = true;

    // Editor Mode
    enum EDITOR_MODE { NONE, EDIT_QUESTION, ADD_QUESTION};
    private EDITOR_MODE editorMode;
    [HideInInspector] public static bool isNewCatalogue;
    [HideInInspector] public static bool isNewTopic;

    // text input mode
    enum INPUT_MODE { NONE, RENAME_CATALOGUE, RENAME_QUESTION, EDIT_ANSWER, EDIT_QUESTION_TEXT, EDIT_TOPIC};
    private INPUT_MODE inputMode = INPUT_MODE.NONE;
    private int editAnswerIndex = -1;

    // text input status
    enum TEXT_INPUT { HIDDEN, VISIBLE, ACCEPT, DECLINE };
    private string currentTextInput = "";

    // user alerts
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private GameObject deleteQuestionAlert;
    [SerializeField] private GameObject deleteCatalogueAlert;
    [SerializeField] private GameObject duplicateQuestionAlert;
    [SerializeField] private GameObject duplicateCatalogueAlert;
    [SerializeField] private GameObject customAlert;
    [SerializeField] private TextMeshProUGUI customAlertTitle;
    [SerializeField] private TextMeshProUGUI customAlertMessage;

    // Scene transition
    private string targetScene = "ContentSelection";

    // Supabase
    private static Supabase.Client _supabase;

    private ICatalogueRepository _catalogueRepo;

    private FetchCataloguesUseCase _cataloguesUseCase;
    private SupabaseRequestUseCase _supabaseRequestUseCase;

    private CatalogueController _catalogueController;


    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Editor")
        {
            print("ERROR [EditorManager.cs:Start()]: Dont use this script in any other scene than 'Editor'.");
        }

        _supabase = SupabaseClientProvider.GetClient();
        _catalogueRepo = new SupabaseCatalogueRepository(_supabase);
        _cataloguesUseCase = new FetchCataloguesUseCase(_catalogueRepo);
        _supabaseRequestUseCase = new SupabaseRequestUseCase(_catalogueRepo);
        _catalogueController = new CatalogueController(_cataloguesUseCase, _supabaseRequestUseCase);

        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        foreach (Button button in answerButtons)
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());

        textInputField.onValueChanged.AddListener(UpdateCurrentInputFieldText);

        if (Global.EditorType == SceneLoader.EditorType.Topic)
        {
            DisplayTopicEditor();
            return;
        }
        else
        {
            currentCatalogue = Global.tmpCatalogue;

            if (currentCatalogue != null)
            {
                invalidStart = false;
            }

            if (invalidStart)
            {
                questionEditor.gameObject.SetActive(false);
                textInputView.gameObject.SetActive(false);
                questionSelectionScrollView.gameObject.SetActive(false);
                emergencyReturnButton.gameObject.SetActive(true);

                string alertTitle = "Warnung";
                string alertMessage = "Etwas ist schiefgelaufen. Kehre zum vorherigen Bildschirm zurück.";
                TriggerCustomAlert(alertTitle, alertMessage);

                return;
            }

            if (isNewCatalogue)
            {
                CreateNewCatalogue();
            }
            else
            {
                DisplayQuestionSelection();
            }
        }
    }

    private void DisplayTopicEditor()
    {
        textInputHeader.text = "Oberthema benennen";
        DisplayTextInputView(INPUT_MODE.EDIT_TOPIC, Global.tmpTopic.name);
    }

    public async void DeleteTopic()
    {
        try
        {
            await _catalogueController.DeleteTopic(currentTopicName);

            targetScene = "EditorMenu";
            textInputHeader.text = "Texteingabe";
            LoadSceneInternal();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void CreateNewCatalogue()
    {
        bool isCatalogueValid = false;
        int i = 2;

        while (!isCatalogueValid)
        {
            if (ContentSelectionHandler.catalogues.Find(catalogue => catalogue.name == currentCatalogue.name) == null)
            {
                isCatalogueValid = true;
                // to do: necessary?
                //ContentSelectionHandler.catalogues.Add(currentCatalogue);
                break;
            }
            else
            {
                currentCatalogue.name = currentCatalogue.name + " " + i;
                i++;
            }
        }
        
        DisplayQuestionSelection();
    }


    /*
    private void DisplayCreateCatalogueOptions()
    {
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(false);
        optionsAddCatalogue.gameObject.SetActive(true);
        editorMode = EDITOR_MODE.NONE;
        inputMode = INPUT_MODE.NONE;
    }
    */

    private void DisplayQuestionSelection()
    {
        CatalogueLabel.text = currentCatalogue.name;

        isPrivateCheckbox.isOn = currentCatalogue.isPrivate;

        if (buttonContainer.transform.childCount > 1)
        {
            for (int i = buttonContainer.transform.childCount - 1; i > 0; i--)
            {
                Destroy(buttonContainer.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < currentCatalogue.questions.Count; i++)
        {
            GameObject questionButton = Instantiate(questionButtonPrefab, buttonContainer);
            questionButton.SetActive(true);

            // display question name on button
            TextMeshProUGUI buttonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = currentCatalogue.questions[i].name;
        }

        //optionsAddCatalogue.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(false);
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(true);
        inputMode = INPUT_MODE.NONE;
        editorMode = EDITOR_MODE.NONE;
    }

    public void toggleIsPrivateCatalogue()
    {
        currentCatalogue.isPrivate = isPrivateCheckbox.isOn;
    }


    private void DisplayQuestionEditor(EDITOR_MODE newMode)
    {
        //optionsAddCatalogue.gameObject.SetActive(false);
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(true);
        editorMode = newMode;
        inputMode = INPUT_MODE.NONE;
    }


    private void DisplayTextInputView(INPUT_MODE newMode, string defaultText = "")
    {
        deleteTopicButton.gameObject.SetActive(newMode == INPUT_MODE.EDIT_TOPIC);

        textInputField.placeholder.GetComponent<TextMeshProUGUI>().text = defaultText;
        textInputField.text = defaultText;

        //optionsAddCatalogue.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(false);
        textInputView.gameObject.SetActive(true);
        inputMode = newMode;
    }

    private void SetQuestionEditor()
    {
        // set question and answer labels
        questionName.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.name;
        questionButtonLabel.text = currentQuestion.text;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtonLabels[i].text = currentQuestion.answers[i].text;
        }
    }

    public void ReturnToCatalogueSelection()
    {
        LoadCatalogueSelection();
    }

    public void ReturnToQuestionSelection()
    {
        if(editorMode == EDITOR_MODE.ADD_QUESTION && (currentTextInput == "" || currentQuestion.name == "Neue Frage"))
        {
            string alertTitle = "Information";
            string alertMessage = "Der aktuelle Fragenname ist nicht gültig.\n\nBitte gib einen anderen Namen ein.";
            TriggerCustomAlert(alertTitle, alertMessage);
            return;
        }

        StoreQuestion();
        DisplayQuestionSelection();
    }



    public void RenameCatalogue()
    {
        DisplayTextInputView(INPUT_MODE.RENAME_CATALOGUE, currentCatalogue.name);
    }

    public async void StoreCatalogue()
    {
        try
        {
            await _catalogueController.UpdateCatalogue(currentCatalogue);
            LoadCatalogueSelection();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

    private void StoreQuestion()
    {
        if (editorMode == EDITOR_MODE.ADD_QUESTION)
        {
            Global.tmpCatalogue.questions.Add(currentQuestion);
            currentCatalogue = Global.tmpCatalogue;
            //catalogueTable.AddQuestion(currentCatalogue.id, currentQuestion);
            //catalogueTable.UpdateCatalogue(currentCatalogue);
            //currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogue.id);
        }

        if (editorMode == EDITOR_MODE.EDIT_QUESTION)
        {
            int tmpQuestionIndex = Global.tmpCatalogue.questions.FindIndex(q => q.id == currentQuestion.id);
            Global.tmpCatalogue.questions[tmpQuestionIndex].name = currentQuestion.name;
            Global.tmpCatalogue.questions[tmpQuestionIndex].text = currentQuestion.text;

            /*
            catalogueTable.UpdateQuestionNameByID(currentQuestion.id, currentQuestion.name);
            catalogueTable.UpdateQuestionTextByID(currentQuestion.id, currentQuestion.text);
            foreach (Answer answer in currentQuestion.answers)
            {
                catalogueTable.UpdateAnswerTextByID(answer.id, answer.text);
            }  
            */
        }
    }

    public async void DeleteCatalogue()
    {
        try
        {
            await _catalogueController.DeleteCatalogue(currentCatalogue.id);

            Global.SetTmpCatalogue(null);

            //catalogueTable.DeleteCatalogueById(currentCatalogue.id);
            LoadCatalogueSelection();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

    public void AddNewQuestion()
    {
        currentTextInput = "";

        var questionIds = currentCatalogue.questions.Select(q => q.id).ToList();
        questionIds.Add(-1);
        var newQuestionId = questionIds.Min() - 1;

        List<Answer> answers = new List<Answer>();
        for (int i = 0; i < 4; i++)
        {
            answers.Add(new(-1, "Antwort " + (i + 1).ToString(), newQuestionId, i == 0));
        }

        currentQuestion = new Question(newQuestionId, "Neuer Fragentext", "Neue Frage", 0, 0, currentCatalogue.id, false, answers, new List<AnswerHistory>());

        SetQuestionEditor();
        DisplayQuestionEditor(EDITOR_MODE.ADD_QUESTION);
    }

    
    public void EditQuestion(TextMeshProUGUI questionButtonLabel)
    {        
        currentQuestion = currentCatalogue.questions.Find(question => question.name == questionButtonLabel.text);

        SetQuestionEditor();
        DisplayQuestionEditor(EDITOR_MODE.EDIT_QUESTION);
    }

    public void RenameQuestion()
    {
        DisplayTextInputView(INPUT_MODE.RENAME_QUESTION, currentQuestion.name);
    }

    public async void DeleteQuestion()
    {
        if (editorMode == EDITOR_MODE.ADD_QUESTION)
        {
            DisplayQuestionSelection();
            return;
        }

        try
        {
            await _catalogueController.DeleteQuestion(currentQuestion.id);
            currentCatalogue.questions.RemoveAll(question => question.id == currentQuestion.id);

            DisplayQuestionSelection();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void EditQuestionText()
    {
        string questionText = questionButtonLabel.text;
        DisplayTextInputView(INPUT_MODE.EDIT_QUESTION_TEXT, questionText);
    }

    public void EditAnswerText(int answerButtonIndex)
    {
        string answerText = answerButtonLabels[answerButtonIndex].text;
        editAnswerIndex = answerButtonIndex;
        DisplayTextInputView(INPUT_MODE.EDIT_ANSWER, answerText);
    }

    public void AcceptTextInput()
    {
        VerifyTextInput();
    }

    public void AbortTextInput()
    {
        switch (inputMode)
        {
            case INPUT_MODE.EDIT_TOPIC:
                targetScene = "EditorMenu";
                textInputHeader.text = "Texteingabe";
                LoadSceneInternal();
                break;
            case INPUT_MODE.RENAME_CATALOGUE:
                DisplayQuestionSelection();
                break;

            case INPUT_MODE.RENAME_QUESTION:
                DisplayQuestionEditor(editorMode);
                break;

            case INPUT_MODE.EDIT_QUESTION_TEXT:
                DisplayQuestionEditor(editorMode);
                break;

            case INPUT_MODE.EDIT_ANSWER: 
                DisplayQuestionEditor(editorMode);
                break;

            default: break;
        }
    }

    public void TriggerAlert(GameObject alert)
    {
        alertPanel.gameObject.SetActive(true);
        alert.gameObject.SetActive(true);
    }

    private void TriggerCustomAlert(string title, string message)
    {
        customAlertTitle.text = title;
        customAlertMessage.text = message;
        alertPanel.gameObject.SetActive(true);
        customAlert.gameObject.SetActive(true);
    }

    public void CloseAlert()
    {
        alertPanel.gameObject.SetActive(false);
        deleteCatalogueAlert.gameObject.SetActive(false);
        deleteQuestionAlert.gameObject.SetActive(false);
        duplicateCatalogueAlert.gameObject.SetActive(false);
        duplicateQuestionAlert.gameObject.SetActive(false);
        customAlert.gameObject.SetActive(false);
    }


    private void LoadCatalogueSelection()
    {
        if (bg != null)
        {
            float timeNeeded = bg.TriggerEndSequence();
            Invoke(nameof(LoadSceneInternal), timeNeeded);
        }
        else
        {
            LoadSceneInternal();
        }
    }


    private void LoadSceneInternal()
    {
        SceneManager.LoadScene(targetScene);
    }


    private async void VerifyTextInput()
    {
        if (inputMode == INPUT_MODE.NONE)
        {
            if (editorMode == EDITOR_MODE.NONE)
            {
                DisplayQuestionSelection();
            } 
            else
            {
                DisplayQuestionEditor(editorMode);
            }
            return;
        };

        if (currentTextInput == "")
        {
            string alertTitle = "Information";
            string alertMessage = "Eine leere Eingabe ist nicht erlaubt.\n\nBitte gib einen Text ein.";
            TriggerCustomAlert(alertTitle, alertMessage);
        }

        switch (inputMode)
        {
            case INPUT_MODE.EDIT_TOPIC:
                bool isExsistingTopic = ContentSelectionHandler.topics.Find(topic => topic.name == currentTextInput) != null;

                if (isExsistingTopic)
                {
                    string alertTitle = "Information";
                    string alertMessage = "Ein Oberthema mit diesem Namen existiert bereits.\n\nBitte wähle einen anderen Namen.";
                    TriggerCustomAlert(alertTitle, alertMessage);
                }
                else
                {
                    try
                    {
                        Models.Topic newTopic = new Models.Topic
                        {
                            Name = currentTextInput
                        };

                        if (currentTopicName != null)
                        {
                            await _catalogueController.UpdateTopic(newTopic, currentTopicName);
                            Topic t = ContentSelectionHandler.topics.Find(topic => topic.name == currentTopicName);
                            t.name = currentTextInput;
                        }
                        else
                        {
                            await _catalogueController.StoreTopic(newTopic);
                            ContentSelectionHandler.topics.Add(new Topic(currentTextInput));
                        }

                        targetScene = "EditorMenu";
                        textInputHeader.text = "Texteingabe";
                        LoadSceneInternal();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                break;

            case INPUT_MODE.RENAME_CATALOGUE:
                if (currentCatalogue.name == currentTextInput)
                {
                    DisplayQuestionSelection();
                    break;
                }
                if (ContentSelectionHandler.catalogues.Find(c => c.name == currentCatalogue.name) != null)
                {
                    TriggerAlert(duplicateCatalogueAlert);
                    break;
                }

                currentCatalogue.name = currentTextInput;
                DisplayQuestionSelection();
                break;

            case INPUT_MODE.RENAME_QUESTION:
                if (currentTextInput == currentQuestion.name)
                {
                    DisplayQuestionEditor(editorMode);
                    break;
                }

                bool isValidQuestionName = true;

                foreach (Question q in currentCatalogue.questions)
                {
                    if (q.name == currentTextInput)
                    {
                        TriggerAlert(duplicateQuestionAlert);
                        isValidQuestionName = false;
                        break;
                    }
                }

                if (isValidQuestionName)
                {
                    currentQuestion.name = currentTextInput;
                    SetQuestionEditor();
                    DisplayQuestionEditor(editorMode);
                }

                break;

            case INPUT_MODE.EDIT_QUESTION_TEXT:
                currentQuestion.text = currentTextInput;
                SetQuestionEditor();
                DisplayQuestionEditor(editorMode);
                break;

            case INPUT_MODE.EDIT_ANSWER:
                currentQuestion.answers[editAnswerIndex].text = currentTextInput;
                SetQuestionEditor();
                DisplayQuestionEditor(editorMode);
                break;

            default: break;
        }
    }



    public void UpdateCurrentInputFieldText(string textInput)
    {
        if (textInput != "")
            currentTextInput = textInput;
    }



    // Import

    /*
    public void OnOpenFileBrowserImportButtonClicked()
    {
        StartCoroutine(OpenFileBrowserAndLoadCatalogue());
    }

    private IEnumerator OpenFileBrowserAndLoadCatalogue()
    {
        FileBrowser.SetFilters(false, ".json");
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Wähle einen Katalog", "Öffnen");

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result);
    }

    void OnFilesSelected(string[] filePaths)
    {
        if (filePaths.Length == 0)
        {
            throw new FileNotFoundException($"{filePaths} size is Zero");
        }

        string path = filePaths[0];

        if (path == null || path.Length == 0)
        {
            throw new FileNotFoundException($"{path} is empty!");
        }

        string customPath = Application.temporaryCachePath + "/" + FileBrowserHelpers.GetFilename(path);
        FileBrowserHelpers.CopyFile(path, customPath);

        if (!File.Exists(customPath))
        {
            throw new FileNotFoundException($"{customPath} does not exist!");
        }

        try
        {
            currentCatalogue = JsonConvert.DeserializeObject<Catalogue>(File.ReadAllText(customPath));
            CreateNewCatalogue();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }
    */

    // Export

    /*
    public void OnOpenFileBrowserExportButtonClicked()
    {
        StartCoroutine(OpenFileBrowserAndSaveCatalogue());
    }

    private IEnumerator OpenFileBrowserAndSaveCatalogue()
    {
        FileBrowser.SetFilters(false, ".json");
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, null, "Wähle einen Speicherort", "Speichern");

        if (FileBrowser.Success)
            SaveCatalogue(FileBrowser.Result);
    }

    void SaveCatalogue(string[] filePaths)
    {
        if (filePaths.Length == 0)
        {
            throw new FileNotFoundException($"{filePaths} size is Zero");
        }

        string path = filePaths[0];

        try
        {
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(currentCatalogue));
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message}");
        }
    }
    */
}

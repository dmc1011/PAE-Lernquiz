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
//using UnityEngine.UIElements;

public class EditorManager : MonoBehaviour
{
    // Screen Elements: Options for adding new Catalogue
    [SerializeField] private GameObject optionsAddCatalogue;

    // Screen Elements: Question Selection
    [SerializeField] private GameObject questionSelectionScrollView;
    [SerializeField] private TextMeshProUGUI CatalogueLabel;
    [SerializeField] private GameObject questionButtonPrefab;      // used for dynamically rendering question buttons
    [SerializeField] private Transform buttonContainer;            // 'content' element of scroll view
    [SerializeField] private GameObject emergencyReturnButton;     // used in case selected catalogue could not be loaded for editor

    // Screen Elements: Question Editor
    [SerializeField] private GameObject questionEditor;
    [SerializeField] private Button questionName;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];

    // Screen Elements: Text Input
    [SerializeField] private GameObject textInputView;
    [SerializeField] private TMP_InputField textInputField;

    [SerializeField] private Background bg = null;


    // Quiz Area Labels
    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();


    // catalogue
    private Catalogue currentCatalogue;
    private CatalogueTable catalogueTable;
    private Question currentQuestion;

    // safety
    private bool invalidStart = true;

    // Editor Mode
    enum EDITOR_MODE { NONE, EDIT_QUESTION, ADD_QUESTION};
    private EDITOR_MODE editorMode;
    [HideInInspector] public static bool isNewCatalogue;

    // text input mode
    enum INPUT_MODE { NONE, RENAME_CATALOGUE, RENAME_QUESTION, EDIT_ANSWER, EDIT_QUESTION_TEXT};
    private INPUT_MODE inputMode = INPUT_MODE.NONE;
    private int editAnswerIndex = -1;


    // text input status
    enum TEXT_INPUT { HIDDEN, VISIBLE, ACCEPT, DECLINE };
    //private TEXT_INPUT textInputStatus = TEXT_INPUT.HIDDEN;
    private string currentTextInput = "";

    // Scene transition
    private string targetScene = "NewGame";

    


    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Catalogues")
        {
            print("ERROR [CataloguesManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }

        // Get components for questionButton
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get components for answer buttons and add them to the lists
        foreach (Button button in answerButtons)
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());

        //ToggleTextInput(textInputStatus);
        textInputField.onValueChanged.AddListener(UpdateCurrentInputFieldText);

        // Use try here if something goes wrong when reading the DB -> This always happens when loading this screen directly and not from Home.
        try
        {
            catalogueTable = SQLiteSetup.Instance.catalogueTable;
            currentCatalogue = Global.tmpCatalogue;

            if (catalogueTable != null && currentCatalogue != null)
            {
                invalidStart = false;
            }
        }
        catch
        {
            print("ERROR: You propably started this screen directly. Start \"Home\" and navigate here instead.");
        }

        if (invalidStart)
        {
            questionEditor.gameObject.SetActive(false);
            textInputView.gameObject.SetActive(false);
            questionSelectionScrollView.gameObject.SetActive(false);
            emergencyReturnButton.gameObject.SetActive(true);
            return;
        }

        if (isNewCatalogue)
        {
            DisplayCreateCatalogueOptions();
        }
        else
        {
            DisplayQuestionSelection();
        }
    }

    public void CreateNewCatalogue()
    {
        catalogueTable.AddCatalogue(currentCatalogue);
        currentCatalogue = catalogueTable.FindCatalogueByName(currentCatalogue.name);
        DisplayQuestionSelection();
    }

    private void DisplayCreateCatalogueOptions()
    {
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(false);
        optionsAddCatalogue.gameObject.SetActive(true);
        editorMode = EDITOR_MODE.NONE;
        inputMode = INPUT_MODE.NONE;
    }

    private void DisplayQuestionSelection()
    {
        CatalogueLabel.text = currentCatalogue.name;

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

        optionsAddCatalogue.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(false);
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(true);
        inputMode = INPUT_MODE.NONE;
        editorMode = EDITOR_MODE.NONE;
    }


    private void DisplayQuestionEditor(EDITOR_MODE newMode)
    {
        optionsAddCatalogue.gameObject.SetActive(false);
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(false);
        questionEditor.gameObject.SetActive(true);
        editorMode = newMode;
        inputMode = INPUT_MODE.NONE;
    }


    private void DisplayTextInputView(INPUT_MODE newMode, string defaultText = "")
    {
        textInputField.placeholder.GetComponent<TextMeshProUGUI>().text = defaultText;
        textInputField.text = defaultText;

        optionsAddCatalogue.gameObject.SetActive(false);
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
        Debug.Log("" + editorMode.ToString());
        Debug.Log("" + currentTextInput);

        if(editorMode == EDITOR_MODE.ADD_QUESTION && (currentTextInput == "" || currentQuestion.name == "Neue Frage"))
        {
            print("Bitte gib einen gültigen Fragennamen ein.");
            return;
        }

        StoreQuestion();
        DisplayQuestionSelection();
    }



    public void RenameCatalogue()
    {
        DisplayTextInputView(INPUT_MODE.RENAME_CATALOGUE, currentCatalogue.name);
    }

    public void StoreCatalogue()
    {
        if (isNewCatalogue)
        {
            if (catalogueTable.FindCatalogueByName(currentCatalogue.name) != null)
            {
                print("Katalogname existiert bereits in DB");
                return;
            }

            catalogueTable.AddCatalogue(currentCatalogue);
        } 
        else
        {
            catalogueTable.UpdateCatalogue(currentCatalogue);
        }

        LoadCatalogueSelection();
    }

    private void StoreQuestion()
    {
        if (editorMode == EDITOR_MODE.ADD_QUESTION)
        {
            catalogueTable.AddQuestion(currentCatalogue.id, currentQuestion);
            catalogueTable.UpdateCatalogue(currentCatalogue);
            currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogue.id);
        }

        if (editorMode == EDITOR_MODE.EDIT_QUESTION)
        {
            catalogueTable.UpdateQuestionNameByID(currentQuestion.id, currentQuestion.name);
            catalogueTable.UpdateQuestionTextByID(currentQuestion.id, currentQuestion.text);
            foreach (Answer answer in currentQuestion.answers)
            {
                catalogueTable.UpdateAnswerTextByID(answer.id, answer.text);
            }  
        }
    }

    public void DeleteCatalogue()
    {
        TriggerSafetyModal();   // To do

        if (isNewCatalogue)
        {
            LoadCatalogueSelection();
            return;
        }

        catalogueTable.DeleteCatalogueById(currentCatalogue.id);
        Global.SetTmpCatalogue(null);

        LoadCatalogueSelection();
    }

    public void AddNewQuestion()
    {
        currentTextInput = "";

        List<Answer> answers = new List<Answer>();
        for (int i = 0; i < 4; i++)
        {
            answers.Add(new(-1, "Antwort " + (i + 1).ToString(), -1, i == 0));
        }

        currentQuestion = new Question(-1, "Neuer Fragentext", "Neue Frage", 0, 0, currentCatalogue.id, false, answers, new List<AnswerHistory>());

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

    public void DeleteQuestion()
    {
        TriggerSafetyModal();   // To do

        if (editorMode == EDITOR_MODE.ADD_QUESTION)
        {
            DisplayQuestionSelection();
            return;
        }

        currentCatalogue.questions.Remove(currentQuestion);
        catalogueTable.DeleteQuestionById(currentQuestion.id);
        DisplayQuestionSelection();
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

    private void TriggerSafetyModal()
    {
        // To do
        // case: delete catalogue
        // case: delete question
        // case: exit scene without saving (for each "BackButton" in Editor scene)
    }

    private void TriggerUserAlert()
    {
        // to do
        // input invalid
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


    private void VerifyTextInput()
    {
        print("Verify text input for \"" + currentTextInput + "\"");

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
            print("Bitte gib eine gültige Texteingabe ein.");
        }

        switch (inputMode)
        {
            case INPUT_MODE.RENAME_CATALOGUE:
                if (currentCatalogue.name == currentTextInput)
                {
                    DisplayQuestionSelection();
                    break;
                }
                if (catalogueTable.FindCatalogueByName(currentTextInput) != null)
                {
                    print("ERROR: There already is a catalogue named \"" + currentTextInput + "\"");
                    TriggerUserAlert();
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
                        print("ERROR: You shall not name two questions the same! " + q.name + " already exists.");
                        TriggerUserAlert();
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
        string path = filePaths[0];

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}");
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            currentCatalogue = JsonConvert.DeserializeObject<Catalogue>(File.ReadAllText(path));
            Debug.Log("Loaded Catalogue succesfully.");
            DisplayQuestionSelection();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }


    // Export

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
}

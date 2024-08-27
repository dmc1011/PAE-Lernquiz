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
    // Screen Elements: Question Selection
    [SerializeField] private GameObject questionSelectionScrollView;
    [SerializeField] private TextMeshProUGUI CatalogueLabel;
    [SerializeField] private GameObject questionButtonPrefab;      // used for dynamically rendering question buttons
    [SerializeField] private Transform buttonContainer;            // 'content' element of scroll view
    [SerializeField] private GameObject emergencyExitButton;       // used in case selected catalogue could not be loaded for editor // to do

    // Screen Elements: Question Editor
    [SerializeField] private GameObject questionEditor;
    [SerializeField] private Button questionName;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];

    // Screen Elements: Text Input
    [SerializeField] private GameObject textInputView;
    [SerializeField] private Button textInputAcceptButton;
    [SerializeField] private Button textInputDeclineButton;
    [SerializeField] private TMP_InputField textInputField;

    [SerializeField] private Background bg = null;


    [SerializeField] private Button[] editButtons = new Button[5]; // Not relevant in which order, this is used to show/hide the buttons when text input is active
    [SerializeField] private Button addTmpCatalogueToDB;


    // Quiz Area Labels
    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();


    // catalogue
    private Catalogue currentCatalogue;
    private CatalogueTable catalogueTable;
    private Question currentQuestion;

    // safety
    private bool questionAddRecursionFailsafe = false; // This "should" never be needed... but. better safe than sorry.
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

    // ???
    private int questionIndexAfterReset = -1;
    private int catalogueIndexAfterReset = -1;

    // Scene transition
    private string targetScene = "NewGame";

    //TODO Delete
    [SerializeField] private TMP_Dropdown catalogueSelection;
    [SerializeField] private TMP_Dropdown questionSelection;

    


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

            DisplayQuestionSelection();
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
            // emergencyExitButton.gameObject.SetActive(true);          // to do
            return;
        }

        // emergencyExitButton.gameObject.SetActive(false);     // to do
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
            buttonLabel.text = currentCatalogue.questions[i].id.ToString(); // to do: .name
        }

        questionEditor.gameObject.SetActive(false);
        textInputView.gameObject.SetActive(false);
        questionSelectionScrollView.gameObject.SetActive(true);
        inputMode = INPUT_MODE.NONE;
        editorMode = EDITOR_MODE.NONE;
    }


    private void DisplayQuestionEditor(EDITOR_MODE newMode)
    {
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
        LoadScene();
    }

    public void ReturnToQuestionSelection()
    {
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
            catalogueTable.AddCatalogue(currentCatalogue);
        } else
        {
            catalogueTable.UpdateCatalogue(currentCatalogue);
        }

        LoadScene();
    }

    private void StoreQuestion()
    {
        if (editorMode == EDITOR_MODE.ADD_QUESTION)
        {
            catalogueTable.AddQuestion(currentCatalogue.id, currentQuestion);
            catalogueTable.UpdateCatalogue(currentCatalogue);
            currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogue.id);
        } else
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

        catalogueTable.DeleteCatalogueById(currentCatalogue.id);
        Global.SetTmpCatalogue(null);

        LoadScene();
    }

    public void AddNewQuestion()
    {
        List<Answer> answers = new List<Answer>();
        for (int i = 0; i < 4; i++)
        {
            answers.Add(new(-1, "Antwort " + (i + 1).ToString(), -1, i == 0));
        }

        currentQuestion = new Question(-1, "Neuer Fragentext", "Neue Frage", 0, currentCatalogue.id, answers, new List<AnswerHistory>());

        SetQuestionEditor();
        DisplayQuestionEditor(EDITOR_MODE.ADD_QUESTION);
    }

    
    public void EditQuestion(TextMeshProUGUI questionButtonLabel)
    {        
        currentQuestion = currentCatalogue.questions.Find(question => question.id.ToString() == questionButtonLabel.text); // to do = .id zu .name

        SetQuestionEditor();
        DisplayQuestionEditor(EDITOR_MODE.EDIT_QUESTION);
    }

    public void RenameQuestion()
    {
        DisplayTextInputView(INPUT_MODE.RENAME_QUESTION, currentQuestion.name);
    }

    public void DeleteQuestion()
    {
        print("EventQuestionDeleteButton");

        TriggerSafetyModal();
        currentCatalogue.questions.Remove(currentQuestion);
        DisplayQuestionSelection();

        TriggerSafetyModal();   // To do
        currentCatalogue.questions.Remove(currentQuestion);
        catalogueTable.DeleteQuestionById(currentQuestion.id);  // To do: notwendig?
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


    private void LoadScene()
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

        switch (inputMode)
        {
            case INPUT_MODE.RENAME_CATALOGUE:
                if (currentTextInput != currentCatalogue.name && catalogueTable.FindCatalogueByName(currentTextInput) != null)
                {
                    print("ERROR: There already is a catalogue named \"" + currentTextInput + "\"");
                    TriggerUserAlert();
                    return;
                }

                currentCatalogue.name = currentTextInput;
                DisplayQuestionSelection();
                break;

            case INPUT_MODE.RENAME_QUESTION:
                if (currentTextInput == currentQuestion.name)
                {
                    DisplayQuestionEditor(editorMode);
                    return;
                }

                foreach (Question q in currentCatalogue.questions)
                {
                    if (q.name == currentTextInput)
                    {
                        print("ERROR: You shall not name two questions the same! " + q.name + " already exists.");
                        TriggerUserAlert();
                        return;
                    }
                }

                currentQuestion.name = currentTextInput;
                SetQuestionEditor();
                DisplayQuestionEditor(editorMode);
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

    private void UpdateCatalogueSelectionTextForCurrentCatalogue()
    {
        catalogueSelection.options[catalogueSelection.value].text = currentCatalogue.name;
        catalogueSelection.captionText.text = currentCatalogue.name;
    }





    private void CatalogueSelectionUpdate()
    {

    }



    public void QuestionSelectionChangedEvent()
    {
                int currentCatalogueId = currentCatalogue.id;
                //catalogueTable.AddQuestion(currentCatalogueId, newQuestion);
                currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogueId);
    }






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
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(File.ReadAllText(path));
            Debug.Log("Loaded Catalogue succesfully.");
            catalogueTable.AddCatalogue(catalogue);
            CatalogueSelectionUpdate();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

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

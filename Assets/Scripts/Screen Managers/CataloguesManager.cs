using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CataloguesManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown catalogueSelection;
    [SerializeField] private TMP_Dropdown questionSelection;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private Button[] editButtons = new Button[5]; // Not relevant in which order, this is used to show/hide the buttons when text input is active
    [SerializeField] private Button addTmpCatalogueToDB;
    [SerializeField] private Button textInputAcceptButton;
    [SerializeField] private Button textInputDeclineButton;
    [SerializeField] private TMP_InputField textInputField;

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();

    private Catalogue currentCatalogue;
    CatalogueTable catalogueTable;

    private bool questionAddRecursionFailsafe = false; // This "should" never be needed... but. better safe than sorry.
    private bool invalidStart = true;

    enum MODE { NONE, RENAME_CATALOGUE, RENAME_QUESTION, EDIT_ANSWER, EDIT_QUESTION};
    private MODE currentMode = MODE.NONE;
    private int editAnswerIndex = -1;

    enum TEXT_INPUT { HIDDEN, VISIBLE, ACCEPT, DECLINE };
    private TEXT_INPUT textInputStatus = TEXT_INPUT.HIDDEN;
    private string currentTextInput = "";

    private int questionIndexAfterReset = -1;
    private int catalogueIndexAfterReset = -1;


    void Start()
    {
        if(SceneManager.GetActiveScene().name != "Catalogues")
        {
            print("ERROR [CataloguesManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }

        // Get components for questionButton
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get components for answer buttons and add them to the lists
        foreach (Button button in answerButtons)
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());

        ToggleTextInput(textInputStatus);
        textInputField.onValueChanged.AddListener(UpdateCurrentInputFieldText);

        // Use try here if something goes wrong when reading the DB -> This always happens when loading this screen directly and not from Home.
        try
        {
            catalogueTable = SQLiteSetup.Instance.catalogueTable;
            invalidStart = false; // catalogue could be loaded -> all fine from here on out
            SetContents();
        }
        catch
        {
            print("ERROR: You propably started this screen directly. Start \"Home\" and navigate here instead.");
        }

    }

    private void SetContents()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;
        // This cascades through every subsequent element and updates everything.
        CatalogueSelectionUpdate();
    }

    public void CatalogueSelectionChangedEvent()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        questionIndexAfterReset = -1;
        string selectedCatalogueName = catalogueSelection.options[catalogueSelection.value].text;
        bool useTmpCatalogue = catalogueSelection.value == catalogueSelection.options.Count - 1;

        if(!useTmpCatalogue)
        {
            currentCatalogue = catalogueTable.FindCatalogueByName(selectedCatalogueName);
            if (currentCatalogue == null)
                useTmpCatalogue = true; // catalogue could not be found. Use a new one per default.
        }

        if(useTmpCatalogue)
        {
            // Count how many "Neuer Fragenkatalog" there are to prevent duplicates
            int newCatalogueNameCounter = 0;
            foreach (Catalogue c in catalogueTable.FindAllCatalogues())
            {
                if(c.name.StartsWith("Neuer Fragenkatalog"))
                    newCatalogueNameCounter++;
            }
            Global.tmpCatalogue ??= new(-1, "Neuer Fragenkatalog" + (newCatalogueNameCounter > 0 ? (" " + newCatalogueNameCounter.ToString()) : ""), -1, 0, new(), new()); // If null -> make a new one.
            currentCatalogue = Global.tmpCatalogue;
            catalogueSelection.value = catalogueSelection.options.Count - 1; // The last index is always "Neu" or the Global.tmpCatalogue
            UpdateCatalogueSelectionTextForCurrentCatalogue();
            
        }

        // You can only add a catalogue if it is the tmpCatalogue. All others already exist in the DB.
        addTmpCatalogueToDB.interactable = useTmpCatalogue;

        QuestionSelectionUpdate();
  
    }

 
    public void QuestionSelectionChangedEvent()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        catalogueIndexAfterReset = -1;
        string selectedQuestionName = questionSelection.options[questionSelection.value].text;

        if (selectedQuestionName == "Neu" && questionAddRecursionFailsafe == false)
        {
            // Add the Question named "Frage N"
            List<Answer> answers = new();
            for (int i = 0; i < 4; i++)
            {
                answers.Add(new(-1, "Antwort " + (i + 1).ToString(), -1, i == 0));
            }

            // Count how many "Neue Frage" there are to prevent duplicates
            int newQuestionCounter = 0;
            foreach(Question question in currentCatalogue.questions)
            {
                if(question.text.StartsWith("Frage"))
                    newQuestionCounter++;
            }

            Question newQuestion = new(-1, "Frage" + (newQuestionCounter + 1).ToString(), "", 0, -1, answers, new List<AnswerHistory>());
            questionIndexAfterReset = questionSelection.value;

            if (currentCatalogue == Global.tmpCatalogue)
            {
                Global.tmpCatalogue.questions.Add(newQuestion);
                currentCatalogue = Global.tmpCatalogue;
            }

            else
            {
                int currentCatalogueId = currentCatalogue.id;
                catalogueTable.AddQuestion(currentCatalogueId, newQuestion);
                currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogueId);
            }

            print(questionIndexAfterReset);

            // This can result in an infinite recursion if the question name at the new questionIndex is somehow "Neu"
            // Just to be safe -> disable recursion for the following call.
            questionAddRecursionFailsafe = true; 
            QuestionSelectionUpdate();
        }
        questionAddRecursionFailsafe = false;

        QuestionAndAnswersSetContents();

    }

    private void HandleTextInput()
    {

        print("HandleTextInput for \"" + currentTextInput + "\"");

        if (currentMode == MODE.NONE) goto switchEnd; // nothing to do here
        if (currentTextInput.Length == 0) goto switchEnd; // nothing to do here

        print(currentMode);
        print("InputText: " + currentTextInput);
        print("InputTextLen: " + currentTextInput.Length);

        switch (currentMode)
        {
            case MODE.RENAME_CATALOGUE:
                {
                    // default name is the current name
                    string newCatalogueName = currentTextInput;
                    string oldCatalogueName = currentCatalogue.name;

                    if (newCatalogueName == oldCatalogueName)
                        goto switchEnd;

                    // these names are invalid or already exist
                    if (newCatalogueName == "Neu" ||
                        catalogueTable.FindCatalogueByName(newCatalogueName) != null ||
                        (Global.tmpCatalogue != null && Global.tmpCatalogue.name == newCatalogueName))
                    {
                        print("ERROR: There already is a catalogue named \"" + newCatalogueName + "\"");
                        goto switchEnd;
                    }

                    // The catalogue is the tmp catalogue
                    if (currentCatalogue == Global.tmpCatalogue)
                    {

                        print("TMP Catalogue " + currentCatalogue.name + " renamed to " + newCatalogueName);
                        Global.tmpCatalogue.name = newCatalogueName;
                        currentCatalogue = Global.tmpCatalogue;
                    }

                    // The catalogue resides within the DB
                    else
                    {
                        int currentCatalogueId = currentCatalogue.id;
                        print("DB Catalogue " + currentCatalogue.name + " renamed to " + newCatalogueName);
                        currentCatalogue.name = newCatalogueName;
                        catalogueTable.UpdateCatalogue(currentCatalogue);
                        currentCatalogue = catalogueTable.FindCatalogueByName(newCatalogueName); // reload the whole catalogue, just to be safe (might be unneccessary)
                    }

                    // Update UI to reflect the new catalogue name.
                    UpdateCatalogueSelectionTextForCurrentCatalogue();

                } break;

            case MODE.RENAME_QUESTION:
                {
                    int currentQuestionIndex = questionSelection.value;
                    string newQuestionName = currentTextInput;
                    string oldQuestionName = questionSelection.options[currentQuestionIndex].text;

                    print("Rename from " + oldQuestionName + " to " + newQuestionName);

                    // nothing changed.
                    if (newQuestionName == oldQuestionName)
                        goto switchEnd;

                    // TODO: Which requirements are there to rename a question?
                    //       names that already exist (in another catalogue) should be valid here.
                    foreach (Question q in currentCatalogue.questions)
                    {
                        if(q.name == newQuestionName)
                        {
                            print("ERROR: You shall not name two questions the same! " + q.name + " already exists.");
                            goto switchEnd;
                        }
                    }

                    questionIndexAfterReset = currentQuestionIndex;

                    if (currentCatalogue == Global.tmpCatalogue)
                    {
                        Global.tmpCatalogue.questions[currentQuestionIndex].name = newQuestionName;
                        currentCatalogue = Global.tmpCatalogue;
                    }

                    else
                    {
                        int currentCatalogueId = currentCatalogue.id;
                        int currentQuestionId = currentCatalogue.questions[currentQuestionIndex].id;
                        print("DB Question " + oldQuestionName + " renamed to " + newQuestionName);
                        catalogueTable.UpdateQuestionNameByID(currentQuestionId, newQuestionName);
                        currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogueId);
                    }


                } break;

            case MODE.EDIT_QUESTION:
                {
                    string newQuestionText = currentTextInput;
                    string oldQuestionText = questionButtonLabel.text;

                    if (newQuestionText == oldQuestionText)
                        goto switchEnd;

                    int currentQuestionIndex = questionSelection.value;

                    questionIndexAfterReset = currentQuestionIndex;

                    if (currentCatalogue == Global.tmpCatalogue)
                    {
                        Global.tmpCatalogue.questions[currentQuestionIndex].text = newQuestionText;
                        currentCatalogue = Global.tmpCatalogue;
                    }
                    
                    else
                    {
                        int currentCatalogueId = currentCatalogue.id;
                        int currentQuestionId = currentCatalogue.questions[currentQuestionIndex].id;
                        catalogueTable.UpdateQuestionTextByID(currentQuestionId, newQuestionText);
                        currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogueId);
                    }


                } break;

                case MODE.EDIT_ANSWER:
                {
                    string newAnswerText = currentTextInput;
                    string oldAnswerText = answerButtonLabels[editAnswerIndex].text;

                    if(newAnswerText == oldAnswerText)
                        goto switchEnd;

                    int currentQuestionIndex = questionSelection.value;
                    questionIndexAfterReset = currentQuestionIndex;

                    if (currentCatalogue == Global.tmpCatalogue)
                    {
                        Global.tmpCatalogue.questions[currentQuestionIndex].answers[editAnswerIndex].text = newAnswerText;
                        currentCatalogue = Global.tmpCatalogue;
                    }

                    else
                    {
                        int currentCatalogueId = currentCatalogue.id;
                        int currentAnswerID = currentCatalogue.questions[currentQuestionIndex].answers[editAnswerIndex].id;
                        catalogueTable.UpdateAnswerTextByID(currentAnswerID, newAnswerText);
                        currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogueId);
                    }

                } break;

        } switchEnd: // Needed here to break multiple nested structures (switch, loops, etc)

        currentMode = MODE.NONE;

        QuestionSelectionUpdate();

    }


    public void EventCatalogueRenameButton()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;
        ToggleTextInput(TEXT_INPUT.VISIBLE, currentCatalogue.name);
        currentMode = MODE.RENAME_CATALOGUE;
    }

    public void EventCatalogueAddButton()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        print("EventCatalogueAddButton");

        if(currentCatalogue != Global.tmpCatalogue)
        {
            print("ERROR: How can you press this button, it is disabled ?!");
            return;
        }

        if(currentCatalogue == null)
        {
            print("ERROR: There is no new catalogue, make a new one first!");
            return;
        }

        // ab in die DB damit
        catalogueTable.AddCatalogue(currentCatalogue);

        // update everything
        currentCatalogue = null;
        Global.tmpCatalogue = null; // reset this to null (the catalogue is now part of the DB)
        CatalogueSelectionUpdate();

    }

    public void EventCatalogueDeleteButton()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        print("EventCatalogueDeleteButton");

        if(currentCatalogue == Global.tmpCatalogue)
        {
            Global.tmpCatalogue = null; // Just delete the tmpCatalogue, it will be set to default once accessed again.
        }
        else
        {
            // TODO: Add some "safety net" here. The user can really easy delete something by accident.
            catalogueTable.DeleteCatalogueById(currentCatalogue.id);
        }

        // update everything
        currentCatalogue = null; // not valid anymore
        CatalogueSelectionUpdate();
    }

    public void EventQuestionRenameButton()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        print("EventQuestionRenameButton");

        if (currentCatalogue == null)
        {
            print("ERROR: How'd that happen?! The current catalogue is null!");
            return;
        }

        string selectedQuestionName = questionSelection.options[questionSelection.value].text;
        ToggleTextInput(TEXT_INPUT.VISIBLE, selectedQuestionName);
        currentMode = MODE.RENAME_QUESTION;
    }

    public void EventQuestionDeleteButton()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        print("EventQuestionDeleteButton");

        Question currentQuestion = currentCatalogue.questions[questionSelection.value];
        questionIndexAfterReset = questionSelection.value - 1; // next displayed question is the question "before" the one that was deleted.

        if (currentCatalogue == Global.tmpCatalogue)
        {
            // trivial löschen. Es gibt ja keine IDs
            Global.tmpCatalogue.questions.Remove(currentQuestion);
            currentCatalogue = Global.tmpCatalogue;
        }

        else
        {
            catalogueTable.DeleteQuestionById(currentQuestion.id);
            int currentCatalogueId = currentCatalogue.id;
            currentCatalogue = catalogueTable.FindCatalogueById(currentCatalogueId);
        }

        QuestionSelectionUpdate();

        // TODO MS:
        // Prompts the user if they are really sure what they are doing.



        // Deletes the Question from the currently selected catalogue.
        // If the question is from tmpCatalogue, no DB-Update required.
        // Else DB-Update required.
    }

    public void EventQuestionEditButton()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;
        print("EventQuestionEditButton");
        string questionText = questionButtonLabel.text;
        ToggleTextInput(TEXT_INPUT.VISIBLE, questionText);
        currentMode = MODE.EDIT_QUESTION;
    }

    public void EventAnswerEditButton(int answerButtonIndex)
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;
        print("EventAnswerEditButton " + answerButtonIndex.ToString());
        string answerText = answerButtonLabels[answerButtonIndex].text;
        ToggleTextInput(TEXT_INPUT.VISIBLE, answerText);
        currentMode = MODE.EDIT_ANSWER;
        editAnswerIndex = answerButtonIndex;
    }

    private void QuestionAndAnswersSetContents()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        if (currentCatalogue.questions.Count == 0)
        {
            // This should never happen. TODO (if a new question is made, it "exists" already)
            // If this ever occurs while editing -> file a report :)
            questionButtonLabel.text = "-";
            foreach (TextMeshProUGUI label in answerButtonLabels)
                label.text = "-";
        }
        else
        {
            Question currentQuestion = currentCatalogue.questions[questionSelection.value];
            questionButtonLabel.text = currentQuestion.text;
            for(int i = 0; i < answerButtons.Length; i++)
            {
                answerButtonLabels[i].text = currentQuestion.answers[i].text;
            }
        }
    }

    private void CatalogueSelectionUpdate()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        // TODO Helena: We only need the catalogue names here.
        // -> sqlAccess.catalogueTable.FindAllCatalogueNames();

        List<Catalogue> catalogues = catalogueTable.FindAllCatalogues();
        catalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < catalogues.Count; i++)
            options.Add(new(catalogues[i].name));
        if (Global.tmpCatalogue != null)
            options.Add(new(Global.tmpCatalogue.name)); // if a global catalogue already exists
        else
            options.Add(new("Neu")); // Option to add a new catalogue
        catalogueSelection.AddOptions(options);
        if (catalogueIndexAfterReset != -1)
            catalogueSelection.value = catalogueIndexAfterReset;

        CatalogueSelectionChangedEvent(); // Update everything that is affected by this update
    }

    private void QuestionSelectionUpdate()
    {
        if (invalidStart) return;
        if (currentMode != MODE.NONE) return;

        questionSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < currentCatalogue.questions.Count; i++)
        {
            if (currentCatalogue.questions[i].name != "")
                options.Add(new(currentCatalogue.questions[i].name));
            else
                options.Add(new("Frage (id:" + (currentCatalogue.questions[i].id).ToString() + ")"));
        }
        options.Add(new("Neu")); // Option to add a new question
        questionSelection.AddOptions(options);
        if(questionIndexAfterReset != -1)
            questionSelection.value = questionIndexAfterReset;
        QuestionSelectionChangedEvent(); // Update everything that is affected by this update
    }

    private void UpdateCatalogueSelectionTextForCurrentCatalogue()
    {
        catalogueSelection.options[catalogueSelection.value].text = currentCatalogue.name;
        catalogueSelection.captionText.text = currentCatalogue.name;
    }

    private void ToggleTextInput(TEXT_INPUT textInput, string defaultText = "")
    {
        textInputField.placeholder.GetComponent<TextMeshProUGUI>().text = defaultText;
        textInputField.text = defaultText;
        catalogueSelection.gameObject.SetActive(textInput != TEXT_INPUT.VISIBLE);
        questionSelection.gameObject.SetActive(textInput != TEXT_INPUT.VISIBLE);
        for(int i = 0; i < editButtons.Length; i++)
            editButtons[i].gameObject.SetActive(textInput != TEXT_INPUT.VISIBLE);
        textInputField.gameObject.SetActive(textInput == TEXT_INPUT.VISIBLE);
        textInputAcceptButton.gameObject.SetActive(textInput == TEXT_INPUT.VISIBLE);
        textInputDeclineButton.gameObject.SetActive(textInput == TEXT_INPUT.VISIBLE);
        textInputStatus = textInput;
    }

    public void UpdateCurrentInputFieldText(string textInput)
    {
        if (textInput != "")
            currentTextInput = textInput;
    }

    public void AcceptTextInputEvent()
    {
        ToggleTextInput(TEXT_INPUT.ACCEPT);
        HandleTextInput();
    }

    public void DeclineTextInputEvent()
    {
        ToggleTextInput(TEXT_INPUT.DECLINE);
        currentMode = MODE.NONE; // Whatever was started -> it is gone now.
    }

}

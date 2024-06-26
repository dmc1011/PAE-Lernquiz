using System.Collections.Generic;
using TMPro;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CataloguesManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown catalogueSelection;
    [SerializeField] private TMP_Dropdown questionSelection;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private Button addTmpCatalogueToDB;
    [SerializeField] private Button textInputAcceptButton;
    [SerializeField] private Button textInputDeclineButton;
    [SerializeField] private TMP_InputField textInputField;

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();

    private SQLiteSetup sqlAccess;
    private Catalogue currentCatalogue;

    private bool questionAddRecursionFailsafe = false; // This "should" never be needed... but. better safe than sorry.
    private bool invalidStart = true;

    enum MODE { NONE, RENAME_CATALOGUE, RENAME_QUESTION, EDIT_ANSWER, EDIT_QUESTION};
    private MODE currentMode = MODE.NONE;

    enum TEXT_INPUT { HIDDEN, VISIBLE, ACCEPT, DECLINE };
    private TEXT_INPUT textInputStatus = TEXT_INPUT.HIDDEN;
    private string currentTextInput = "";

    private bool useTmpCatalogue = false; // Use the Global.tmpCatalogue instead of the ones from the DB
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

        try
        {
            sqlAccess = SQLiteSetup.Instance;
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
        // This cascades through every subsequent element and updates everything.
        CatalogueSelectionUpdate();
    }

    public void CatalogueSelectionChangedEvent()
    {
        if (invalidStart) return;
        questionIndexAfterReset = -1;
        string selectedCatalogueName = catalogueSelection.options[catalogueSelection.value].text;
        useTmpCatalogue = catalogueSelection.value == catalogueSelection.options.Count - 1;

        if(!useTmpCatalogue)
        {
            currentCatalogue = sqlAccess.catalogueTable.FindCatalogueByName(selectedCatalogueName);
            if (currentCatalogue == null)
                useTmpCatalogue = true; // catalogue could not be found. Use a new one per default.
        }

        if(useTmpCatalogue)
        {
            // Count how many "Neuer Fragenkatalog" there are to prevent duplicates
            int newCatalogueNameCounter = 0;
            foreach (Catalogue c in sqlAccess.catalogueTable.FindAllCatalogues())
            {
                if(c.name.StartsWith("Neuer Fragenkatalog"))
                    newCatalogueNameCounter++;
            }
            Global.tmpCatalogue ??= new(-1, "Neuer Fragenkatalog" + (newCatalogueNameCounter > 0 ? (" " + newCatalogueNameCounter.ToString()) : ""), new()); // If null -> make a new one.
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
        catalogueIndexAfterReset = -1;
        string selectedQuestionName = questionSelection.options[questionSelection.value].text;

        if (selectedQuestionName == "Neu" && questionAddRecursionFailsafe == false)
        {
            // Add the Question named "Frage N"
            Answer defaultAnswer = new(-1, "", -1, false);
            List<Answer> answers = new List<Answer>();
            for (int i = 0; i < 4; i++)
            {
                defaultAnswer.text = "Antwort " + (i + 1).ToString();
                answers.Add(defaultAnswer);
            }

            // Count how many "Neue Frage" there are to prevent duplicates
            int newQuestionCounter = 0;
            foreach(Question question in currentCatalogue.questions)
            {
                if(question.text.StartsWith("Frage"))
                    newQuestionCounter++;
            }

            Question newQuestion = new(-1, "Frage" + (newQuestionCounter + 1).ToString(), -1, answers);

            currentCatalogue.questions.Add(newQuestion);
            questionIndexAfterReset = currentCatalogue.questions.IndexOf(newQuestion);

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

        if (currentMode == MODE.NONE) goto EndSelect; // nothing to do here
        if (currentTextInput.Length == 0) goto EndSelect; // nothing to do here

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

                    print("Rename from " + oldCatalogueName + " to " + newCatalogueName);

                    // nothing changed.
                    if (newCatalogueName == oldCatalogueName)
                        goto EndSelect;

                    // these names are invalid or already exist
                    if (newCatalogueName == "Neu" ||
                        sqlAccess.catalogueTable.FindCatalogueByName(newCatalogueName) != null ||
                        (Global.tmpCatalogue != null && Global.tmpCatalogue.name == newCatalogueName))
                    {
                        print("ERROR: There already is a catalogue named \"" + newCatalogueName + "\"");
                        goto EndSelect;
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
                        print("DB Catalogue " + currentCatalogue.name + " renamed to " + newCatalogueName);
                        sqlAccess.catalogueTable.UpdateCatalogueById(currentCatalogue.id, newCatalogueName);
                        currentCatalogue = sqlAccess.catalogueTable.FindCatalogueByName(newCatalogueName); // reload the whole catalogue, just to be safe (might be unneccessary)
                    }

                    // Update UI to reflect the new catalogue name.
                    UpdateCatalogueSelectionTextForCurrentCatalogue();

                } break;

            case MODE.RENAME_QUESTION:
                {

                    // TODO MS:
                    // Currently not doable because questions currently have no name.

                    // Promts the user for a name.
                    // This name is given to the currently selected question.
                    // If the question is from tmpCatalogue, no DB-Update required.
                    // Else DB-Update required.

                    string newQuestionName = currentTextInput;
                    string oldQuestionName = questionSelection.options[questionSelection.value].text;

                    print("Rename from " + oldQuestionName + " to " + newQuestionName);

                    // nothing changed.
                    if (newQuestionName == oldQuestionName)
                        goto EndSelect;

                    // TODO: Which requirements are there to rename a question?
                    //       names that already exist (in another catalogue) should be valid here.
                    foreach (Question q in currentCatalogue.questions)
                    {
                        if(q.name == newQuestionName)
                        {
                            print("ERROR: You shall not name two questions the same! " + q.name + " already exists.");
                            goto EndSelect;
                        }
                    }

                    
                    if (currentCatalogue == Global.tmpCatalogue)
                    {
                        Global.tmpCatalogue.questions[questionSelection.value].name = newQuestionName;
                        currentCatalogue = Global.tmpCatalogue;
                        questionIndexAfterReset = questionSelection.value;
                    }

                    else
                    {
                        print("TODO");
                    }


                }
                break;
        }

        QuestionSelectionUpdate();

        // Needed here to break multiple nested structures
        EndSelect:

        currentMode = MODE.NONE;



    }


    public void EventCatalogueRenameButton()
    {
        if (invalidStart) return;
        ToggleTextInput(TEXT_INPUT.VISIBLE, currentCatalogue.name);
        currentMode = MODE.RENAME_CATALOGUE;
    }

    public void EventCatalogueAddButton()
    {
        if (invalidStart) return;

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
        sqlAccess.AddCatalogue(currentCatalogue);

        // update everything
        currentCatalogue = null;
        Global.tmpCatalogue = null; // reset this to null (the catalogue is now part of the DB)
        CatalogueSelectionUpdate();

    }

    public void EventCatalogueDeleteButton()
    {
        if (invalidStart) return;
        print("EventCatalogueDeleteButton");

        if(currentCatalogue == Global.tmpCatalogue)
        {
            Global.tmpCatalogue = null; // Just delete the tmpCatalogue, it will be set to default once accessed again.
        }
        else
        {
            // TODO: Add some "safety net" here. The user can really easy delete something by accident.
            sqlAccess.catalogueTable.DeleteCatalogueById(currentCatalogue.id);
        }

        // update everything
        currentCatalogue = null; // not valid anymore
        CatalogueSelectionUpdate();
    }

    public void EventQuestionRenameButton()
    {
        if (invalidStart) return;
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
        print("EventQuestionDeleteButton");

        // TODO MS:
        // Prompts the user if they are really sure what they are doing.



        // Deletes the Question from the currently selected catalogue.
        // If the question is from tmpCatalogue, no DB-Update required.
        // Else DB-Update required.
    }

    public void EventQuestionEditButton()
    {
        if (invalidStart) return;
        print("EventQuestionEditButton");
        // Prompts the user with a text-input-field where the current question is written out and editable.
        // Closing the prompt will update the question text.
        // If the question is from tmpCatalogue, no DB-Update required.
        // Else DB-Update required.
    }

    public void EventAnswerEditButton(int answerButtonIndex)
    {
        if (invalidStart) return;
        print("EventAnswerEditButton " + answerButtonIndex.ToString());
        // Prompts the user with a text-input-field where the selected answer is written out and editable.
        // Closing the prompt will update the question text.
        // If the question is from tmpCatalogue, no DB-Update required.
        // Else DB-Update required.
    }

    private void QuestionAndAnswersSetContents()
    {
        if (invalidStart) return;
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

        // TODO Helena: We only need the catalogue names here.
        // -> sqlAccess.catalogueTable.FindAllCatalogueNames();

        List<Catalogue> catalogues = sqlAccess.catalogueTable.FindAllCatalogues();
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
        questionSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < currentCatalogue.questions.Count; i++)
        {
            if (currentCatalogue.questions[i].name != "")
                options.Add(new(currentCatalogue.questions[i].name));
            else
                options.Add(new("Frage " + (i + 1).ToString()));
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

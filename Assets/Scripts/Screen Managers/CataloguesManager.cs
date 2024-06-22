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

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();

    CatalogueTable catalogueTable;
    Catalogue currentCatalogue;
    List<Catalogue> catalogues;

    // This "should" never be needed... but. better safe than sorry.
    bool questionAddRecursionFailsafe = false;
    bool invalidStart = true;

    bool useTmpCatalogue = false; // Use the Global.tmpCatalogue instead of the ones from the DB
    int questionIndexAfterReset = -1;
    int catalogueIndexAfterReset = -1;


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

        try
        {
            catalogueTable = SQLiteSetup.Instance.catalogueTable;
            catalogues = catalogueTable.FindAllCatalogues();
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
        useTmpCatalogue = selectedCatalogueName == "Neu";

        if(useTmpCatalogue)
            currentCatalogue = Global.tmpCatalogue;
        else
            currentCatalogue = catalogueTable.FindCatalogueByName(selectedCatalogueName);

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
                if(question.text[.."Frage".Length] == "Frage")
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

    public void EventCatalogueRenameButton()
    {
        if (invalidStart) return;
        print("EventCatalogueRenameButton");
        // Prompts the user for a name.
        // Sets the name of the current catalogue.
        // If the catalogue is NOT the tmpCatalogue: Updates DB with a new name for this catalogue.
        // Else: All fine.
    }

    public void EventCatalogueAddButton()
    {
        if (invalidStart) return;
        print("EventCatalogueAddButton");
        // Promts the user for a name.
        // Adds the tmpCatalogue to the DB with the name given by the user
        // Resets the tmpCatalogue to default.
    }

    public void EventCatalogueDeleteButton()
    {
        if (invalidStart) return;
        print("EventCatalogueDeleteButton");
        // Prompts the user if they are really sure what they are doing.
        // Moves the currently selected, existing catalogue (not for the tmpCatalogue) to the recycle bin.
        // Finally deleting maybe on the settings screen?
    }

    public void EventQuestionRenameButton()
    {
        if (invalidStart) return;
        print("EventQuestionRenameButton");
        // Promts the user for a name.
        // This name is given to the currently selected question.
        // If the question is from tmpCatalogue, no DB-Update required.
        // Else DB-Update required.
    }

    public void EventQuestionDeleteButton()
    {
        if (invalidStart) return;
        print("EventQuestionDeleteButton");
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
            // This should never happen. TODO (if a new question made, it "exists" already)
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
        catalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < catalogues.Count; i++)
            options.Add(new(catalogues[i].name));
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
            options.Add(new("Frage " + (i + 1).ToString()));
        options.Add(new("Neu")); // Option to add a new question
        questionSelection.AddOptions(options);
        if(questionIndexAfterReset != -1)
            questionSelection.value = questionIndexAfterReset;

        QuestionSelectionChangedEvent(); // Update everything that is affected by this update
    }

}

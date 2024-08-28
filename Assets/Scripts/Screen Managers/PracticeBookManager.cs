using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PracticeBookManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private QuizAreaManager quizAreaManager;
    [SerializeField] private Button nextButton;
    [SerializeField] private ButtonNavigation nextButtonNavigation;

    private TextMeshProUGUI nextButtonLabel;
    private Catalogue currentCatalogue;
    private List<Question> questions;
    private List<Question> allQuestions;
    private int nextQuestionIndex = 0;
    private CatalogueTable catalogueTable;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "PracticeBook");
        PlayerPrefs.Save();

        DataManager.ClearResults();
        catalogueTable = SQLiteSetup.Instance.catalogueTable;

        nextButtonLabel = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get current catalogue
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        allQuestions = currentCatalogue.questions;
        questions = allQuestions.Where(q => q.enabledForPractice).ToList();
        nextButton.interactable = false;

        // Display the first question
        DisplayNextQuestion();
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        if (nextQuestionIndex >= questions.Count)
        {
            LoadNextScene();
            return;
        }

        Question nextQuestion = questions[nextQuestionIndex];
        quizAreaManager.ResetContents();
        quizAreaManager.RandomizePositions();
        quizAreaManager.SetContents(nextQuestion);

        if (nextQuestionIndex == questions.Count - 1)
            nextButtonLabel.text = "Beenden";

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {allQuestions.FindIndex(q => q == nextQuestion) + 1}";
        nextButton.interactable = false;
        nextQuestionIndex += 1;
    }

    public void EventButtonPressedCallback(QuizAreaManager.ButtonID button)
    {
        switch (button)
        {
            case QuizAreaManager.ButtonID.Q:
                {
                    // MS: There is currently no logic involved in pressing the question button.
                    // But the event is forwarded for potential later use.
                }
                break;

            case QuizAreaManager.ButtonID.A: // MS: I wanted to write it "exactly this way" to support
            case QuizAreaManager.ButtonID.B: // the case where we have different logic for different buttons.
            case QuizAreaManager.ButtonID.C: // Currently it's all the same. I know.
            case QuizAreaManager.ButtonID.D: // This also filters any unwanted values of "button" if we add something in the future.
                {
                    int questionIndex = allQuestions.FindIndex(q => q == questions[nextQuestionIndex - 1]);
                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);
                    nextButton.interactable = true;
                }
                break;
        }

    }

    public void LoadNextScene()
    {
        nextButtonNavigation.LoadScene("Evaluation");
    }
}


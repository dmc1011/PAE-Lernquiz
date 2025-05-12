using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataManager;
using static Global;
using Entities;

public class DailyTaskManager : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private QuizAreaManager quizAreaManager;
    [SerializeField] private Button nextButton;

    private TextMeshProUGUI nextButtonLabel;
    private bool isQuizOver = false;
    private int questionCount = 0;
    private int questionLimit;
    private Catalogue currentCatalogue;
    private int nextQuestionIndex;

    private DailyTaskHistoryTable dailyTaskHistoryTable;

    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "DailyTask");
        PlayerPrefs.Save();

        // Get components and set default values
        nextButtonLabel = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get current catalogue
        currentCatalogue = Global.CurrentDailyTask.catalogue;
        questionLimit = Global.CurrentDailyTask.questionLimit;
        nextButton.interactable = false;

        dailyTaskHistoryTable = SQLiteSetup.Instance.dailyTaskHistoryTable;
        dailyTaskHistoryTable.AddDailyTaskHistory();
        
        // Display first question
        DisplayNextQuestion();
    }

    public void DisplayNextQuestion()
    {
        if (isQuizOver || questionCount >= questionLimit)
        {
            Global.CurrentDailyTask.answers = DataManager.QuestionResults;
            PlayerLevel.GainXp(PlayerLevel.dailyTaskXp);
            LoadNextScene();
            return;
        }

        nextQuestionIndex = Global.CurrentDailyTask.questions[questionCount];
        Question nextQuestion = currentCatalogue.questions[nextQuestionIndex];

        quizAreaManager.ResetContents();
        quizAreaManager.RandomizePositions();
        quizAreaManager.SetContents(nextQuestion);

        // The "last" question is at UBound - 1. The "Beenden" must be shown one question earlier.
        if (questionCount == questionLimit - 1)
            nextButtonLabel.text = "Beenden";

        int questionIndex = CurrentDailyTask.catalogue.questions.FindIndex(q => q == nextQuestion);
        Fragenummer.text = "Frage " + (questionCount + 1) + "/" + questionLimit + "\n" + currentCatalogue.name + ", " + "Frage " + (questionIndex + 1);
        nextButton.interactable = false;
        questionCount += 1; // questionCount will be 0 when first question is displayed

        // Quiz will be considered over as soon as last question is displayed
        if (questionCount >= questionLimit) 
        {
            isQuizOver = true;
        }
    }

    public void SaveAnswerInPlayerPrefs(int questionIndex, int answerIndex, Catalogue catalogue)
    {
        DataManager.QuestionResult questionResult = new DataManager.QuestionResult(questionIndex, answerIndex, catalogue);
        PlayerPrefs.SetString($"dailyQuestion{questionCount}", questionResult.questionText);
        PlayerPrefs.SetString($"dailyAnswerA{questionCount}", questionResult.answerTexts[0]);
        PlayerPrefs.SetString($"dailyAnswerB{questionCount}", questionResult.answerTexts[1]);
        PlayerPrefs.SetString($"dailyAnswerC{questionCount}", questionResult.answerTexts[2]);
        PlayerPrefs.SetString($"dailyAnswerD{questionCount}", questionResult.answerTexts[3]);
        PlayerPrefs.SetString($"dailyAnswer{questionCount}", questionResult.selectedAnswerText);
        PlayerPrefs.SetInt($"dailyAnswerCorrect{questionCount}", questionResult.isCorrect ? 1 : 0);
        PlayerPrefs.SetInt($"dailyQuestionId{questionCount}", questionResult.questionId);
        PlayerPrefs.Save();
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
                    // in contrast to LinearQuizManager nextQuestionIndex is not update at this point and still valid
                    int questionIndex = nextQuestionIndex;
                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);
                    SaveAnswerInPlayerPrefs(questionIndex, (int)button, currentCatalogue);
                    nextButton.interactable = true;
                }
                break;
        }

    }
      // Update is called once per frame
    public void LoadNextScene()
    {
        SceneManager.LoadScene("Evaluation");
    }
}

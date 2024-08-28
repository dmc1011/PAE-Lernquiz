using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationManager : MonoBehaviour
{

    [SerializeField] private EvaluationButton evaluationButtonPrefab;
    [SerializeField] private GameObject NewGameButton;
    [SerializeField] private EvaluationStatistics evaluationStatistics;
    [SerializeField] private Transform scrollTransform;
    [SerializeField] private SideMenu sideMenu;
    [SerializeField] private QuizAreaManager quizAreaManager;

    private List<EvaluationButton> evaluationButtons = new();
    private List<DataManager.QuestionResult> questionResults = new();
    //private string evaluationFor;
    private Global.GameMode gameMode;
    private DailyTaskHistoryTable dailyTaskHistoryTable;

    void Start()
    {
        //evaluationFor = PlayerPrefs.GetString("evaluationFor");
        gameMode = Global.CurrentQuestionRound.gameMode;
        Debug.Log(gameMode);
        dailyTaskHistoryTable = SQLiteSetup.Instance.dailyTaskHistoryTable;

        switch (gameMode) {
            case Global.GameMode.DailyTask:
                NewGameButton.SetActive(false);
                dailyTaskHistoryTable.SetTodaysTaskToCompleted();
                PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "true");
                PlayerPrefs.Save();
                LoadResults(GetDailyTaskResults());
                break;
            case Global.GameMode.LinearQuiz:
            case Global.GameMode.RandomQuiz:
            case Global.GameMode.PracticeBook:
                LoadResults(DataManager.QuestionResults);
                break;
            default:
                print("ERROR: EvaluationManager - Invalid Game Mode for evaluation.");
                break;
        }
    }

    private List<DataManager.QuestionResult> GetDailyTaskResults()
    {
        List<DataManager.QuestionResult> results = new();
        for (int count = 1; count <= Global.DailyTaskSize; count++)
        {
            List<string> allAnswerTexts = new() {
                PlayerPrefs.GetString($"dailyAnswerA{count}"),
                PlayerPrefs.GetString($"dailyAnswerB{count}"),
                PlayerPrefs.GetString($"dailyAnswerC{count}"),
                PlayerPrefs.GetString($"dailyAnswerD{count}")
            };
            DataManager.QuestionResult result = new()
            {
                isCorrect = PlayerPrefs.GetInt($"dailyAnswerCorrect{count}") == 1,
                questionText = PlayerPrefs.GetString($"dailyQuestion{count}"),
                selectedAnswerText = PlayerPrefs.GetString($"dailyAnswer{count}"),
                answerTexts = allAnswerTexts
            };
            results.Add(result);
        }
        return results;
    }

    private void LoadResults(List<DataManager.QuestionResult> results)
    {
        questionResults = results;

        int numAnswers = results.Count;
        int numCorrectAnswers = 0;
        { // Scope this for i
            int i = 0;
            foreach (var result in questionResults)
            {
                EvaluationButton entry = Instantiate(evaluationButtonPrefab, scrollTransform);
                entry.Set(i + 1, result.isCorrect, result.questionText);
                numCorrectAnswers += result.isCorrect ? 1 : 0;
                int ownVariableForI = i;
                entry.GetComponent<Button>().onClick.AddListener(delegate { EvaluationButtonPressed(ownVariableForI); });
                evaluationButtons.Add(entry);
                i++;
            }
        }
        evaluationStatistics.Set(numAnswers, numCorrectAnswers);
    }

    public void EvaluationButtonPressed(int buttonIndex)
    {
        if(questionResults.Count == 0) { return; }
        quizAreaManager.SetContents(questionResults[buttonIndex]);
        sideMenu.ToggleMenu();
    }

    public void EventButtonPressedCallback(QuizAreaManager.ButtonID button)
    {
        // Note: Nothing happens if someone presses a button in the QuizArea this is intended.
        // print("Button Pressed: " + (int)button);
    }

}

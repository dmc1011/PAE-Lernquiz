using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationManager : MonoBehaviour
{

    [SerializeField] private EvaluationButton evaluationButtonPrefab;
    [SerializeField] private EvaluationStatistics evaluationStatistics;
    [SerializeField] private Transform scrollTransform;
    [SerializeField] private SideMenu sideMenu;
    [SerializeField] private QuizAreaManager quizAreaManager;

    private List<EvaluationButton> evaluationButtons = new();
    private List<DataManager.QuestionResult> questionResults = new();
    private string evaluationFor;

    void Start()
    {
        evaluationFor = PlayerPrefs.GetString("evaluationFor");

        if(evaluationFor.Equals("DailyTask"))
        {
            LoadResults(GetQuestionResultsFromDailyTask());
        }
        else if (evaluationFor.Equals("LinearQuiz"))
        {
            LoadResults(DataManager.QuestionResults);
        }
        else if (evaluationFor.Equals("RandomQuiz"))
        {
            LoadResults(DataManager.QuestionResults);
        }
        else
        {
            print("ERROR: Evaluation for " + evaluationFor + " unknwon.");
        }
    }

    private List<DataManager.QuestionResult> GetQuestionResultsFromDailyTask()
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

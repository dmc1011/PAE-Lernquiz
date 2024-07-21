using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class DailyTaskEvaluationManager : MonoBehaviour
{
    [SerializeField] private Transform resultContainer;
    [SerializeField] private GameObject resultPrefab;
    void Start()
    {
        if (PlayerPrefs.GetString(Global.IsDailyTaskCompletedKey) == "false")
        {
            PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "true");
            PlayerPrefs.Save();
        }
        DisplayResults();   
    }

    private void DisplayResults()
    {
        List<DataManager.QuestionResult> results = Global.CurrentDailyTask.answers;

        for (int count = 1; count <= Global.DailyTaskSize; count++)
        {
            GameObject resultEntry = Instantiate(resultPrefab, resultContainer);
            EvaluationTableContent evaluationTableContent = resultEntry.GetComponent<EvaluationTableContent>();
            evaluationTableContent.questionText.text = PlayerPrefs.GetString($"dailyQuestion{count}");
            evaluationTableContent.answerText.text = PlayerPrefs.GetString($"dailyAnswer{count}");
            evaluationTableContent.correctText.text = PlayerPrefs.GetInt($"dailyAnswerCorrect{count}") == 1 ? "Richtig" : "Falsch";
        }
    }
}

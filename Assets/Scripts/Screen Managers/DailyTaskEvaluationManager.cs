using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class DailyTaskEvaluationManager : MonoBehaviour
{
    [SerializeField] private Transform resultContainer;
    [SerializeField] private GameObject resultPrefab;
    void Start()
    {
        Debug.Log("Enter Scene");
        if (PlayerPrefs.GetString(Global.IsDailyTaskCompletedKey) == "false")
        {
            PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "true");
            PlayerPrefs.Save();
        }
        DisplayResults();   
    }

    private void DisplayResults()
    {
        for (int count = 1; count <= Global.DailyTaskSize; count++)
        {
            Debug.Log(count);
            GameObject resultEntry = Instantiate(resultPrefab, resultContainer);
            EvaluationTableContent evaluationTableContent = resultEntry.GetComponent<EvaluationTableContent>();
            evaluationTableContent.questionText.text = PlayerPrefs.GetString($"dailyQuestion{count}");
            evaluationTableContent.answerText.text = PlayerPrefs.GetString($"dailyAnswer{count}");
            evaluationTableContent.correctText.text = PlayerPrefs.GetInt($"dailyAnswerCorrect{count}") == 1 ? "Richtig" : "Falsch";

            Debug.Log(PlayerPrefs.GetString($"dailyQuestion{count}"));
            Debug.Log(PlayerPrefs.GetString($"dailyAnswer{count}"));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
    [SerializeField] private Transform resultContainer;
    [SerializeField] private GameObject resultPrefab;
    void Start()
    {
        DisplayResults();   
    }

    private void DisplayResults()
    {
        List<DataManager.QuestionResult> results = DataManager.QuestionResults;

        foreach (var result in results)
        {
            GameObject resultEntry = Instantiate(resultPrefab, resultContainer);
            EvaluationTableContent evaluationTableContent = resultEntry.GetComponent<EvaluationTableContent>();

            evaluationTableContent.QuestionText.text = result.questionText;
            evaluationTableContent.AnswerText.text = result.givenAnswer;
            evaluationTableContent.CorrectText.text = result.isCorrect ? "Richtig" : "Falsch";
        }
    }
}

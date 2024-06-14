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

            // TODO: as soon as images are supported we might need to adapt the prefab
            evaluationTableContent.questionText.text = result.questionText;
            evaluationTableContent.answerText.text = result.answerText;
            evaluationTableContent.correctText.text = result.isCorrect ? "Richtig" : "Falsch";
        }
    }
}

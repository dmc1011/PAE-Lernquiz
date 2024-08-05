using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{

    [SerializeField] private EvaluationButton evaluationButtonPrefab;
    [SerializeField] private EvaluationStatistics evaluationStatistics;
    [SerializeField] private Transform scrollTransform;

    void Start()
    {
        DisplayResults();   
    }

    private void DisplayResults()
    {
        List<DataManager.QuestionResult> results = DataManager.QuestionResults;

        int numAnswers = results.Count;
        int numCorrectAnswers = 0;

        { // Scope this for i
            int i = 0;
            foreach (var result in results)
            {
                EvaluationButton entry = Instantiate(evaluationButtonPrefab, scrollTransform);
                entry.Set(i + 1, result.isCorrect, result.questionText);
                numCorrectAnswers += result.isCorrect ? 1 : 0;
                i++;
            }
        }

        evaluationStatistics.Set(numAnswers, numCorrectAnswers);

    }
}

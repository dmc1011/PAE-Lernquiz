using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] private GameObject catalogueStatisticPanel;
    [SerializeField] private GameObject dailyTaskStatisticPanel;
    [SerializeField] private TextMeshProUGUI catalogueNameDisplay;
    [SerializeField] private TextMeshProUGUI catalogueLevelDisplay;
    [SerializeField] private TextMeshProUGUI timeValuesDisplay;
    [SerializeField] private TextMeshProUGUI averageValuesDisplay;
    [SerializeField] private BarConfig barConfig;
    [SerializeField] private Transform scrollTransform;
    [SerializeField] private GameObject catalogueName;
    [SerializeField] private GameObject barProgress;
    [SerializeField] private GameObject timeStatistic;
    [SerializeField] private GameObject averageAnswers;

    public static bool isDailyTaskStatistic;

    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private AnswerHistoryTable answerHistoryTable;
    private CatalogueTable catalogueTable;
    private List<CatalogueSessionHistory> catalogueSessionHistories;
    private Catalogue currentCatalogue;

    // average statistics
    private int correctAnswersCount = 0;
    private float averageAnswersCount = 0.0f;

    //time statistics
    private int currentRunDuration = 0;
    private int averageRunDuration = 0;
    private int totalDuration = 0;
    private string totalDurationUnit = "";
    private string currentRunDurationUnit = "";
    private string averageRunDurationUnit = "";

    // current session
    int isCorrectCount = 0;
    int isFalseCount = 0;
    int notAnsweredCount;

    // Start is called before the first frame update
    void Start()
    {
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
        answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;
        catalogueTable = SQLiteSetup.Instance.catalogueTable;

        notAnsweredCount = currentCatalogue.questions.Count;

        if (isDailyTaskStatistic)
        {
            catalogueStatisticPanel.gameObject.SetActive(false);
            dailyTaskStatisticPanel.gameObject.SetActive(true);
        }
        else 
        {
            dailyTaskStatisticPanel.gameObject.SetActive(false);
            catalogueName.transform.SetParent(scrollTransform);
            barProgress.transform.SetParent(scrollTransform);
            averageAnswers.transform.SetParent(scrollTransform);
            timeStatistic.transform.SetParent(scrollTransform);
            catalogueNameDisplay.text = Global.CurrentQuestionRound.catalogue.name;
            catalogueLevelDisplay.text = "Stufe " + Global.CurrentQuestionRound.catalogue.sessionCount.ToString();

            catalogueSessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(currentCatalogue.id);
            SetStatistics();
        }
    }

    private string GetUnit(int seconds)
    {
        return seconds > 60 ? " Minuten" : " Sekunden";
    }

    private void SetStatistics()
    {
        CurrentSessionStatistic();
        TimeStatistics();
        AverageCorrectAnswersInCatalogue();
    }

    private void CurrentSessionStatistic()
    {
        if (catalogueSessionHistories.Count > 0)
        {
            CatalogueSessionHistory session = catalogueSessionHistories[0];
            List<AnswerHistory> answerHistories = answerHistoryTable.FindAnswerHistoryBySessionId(session.id);
            foreach (var answerHistory in answerHistories)
            {
                bool wasCorrect = answerHistory.wasCorrect;
                if (wasCorrect)
                    isCorrectCount++;
                else
                    isFalseCount++;

                notAnsweredCount--;
            }
        }

        barConfig.SetValue(isCorrectCount, isFalseCount, notAnsweredCount);
    }

    private void TimeStatistics()
    {
        if (catalogueSessionHistories.Count > 0)
        {
            currentRunDuration = catalogueSessionHistories[0].timeSpent;
            currentRunDuration = currentRunDuration > 60 ? currentRunDuration / 60 : currentRunDuration;

            int totalDurationSum = catalogueSessionHistories.Sum(history => history.timeSpent);
            averageRunDuration = totalDurationSum / catalogueSessionHistories.Count;
            averageRunDuration = averageRunDuration > 60 ? averageRunDuration / 60 : averageRunDuration;
        }

        totalDuration = currentCatalogue.totalTimeSpent > 60 ? currentCatalogue.totalTimeSpent / 60 : currentCatalogue.totalTimeSpent;
        totalDurationUnit = GetUnit(currentCatalogue.totalTimeSpent);
        currentRunDurationUnit = GetUnit(currentRunDuration);
        averageRunDurationUnit = GetUnit(averageRunDuration);

        timeValuesDisplay.text = $"{currentRunDuration}{currentRunDurationUnit}\n\n{averageRunDuration}{averageRunDurationUnit}\n\n{totalDuration}{totalDurationUnit}";
    }

    private void AverageCorrectAnswersInCatalogue()
    {
        int totalAnswers = 0;

        List<Question> questions = catalogueTable.FindQuestionsByCatalogueId(currentCatalogue.id);

        foreach (var question in questions)
        {
            correctAnswersCount += question.correctAnsweredCount;
            totalAnswers += question.totalAnsweredCount;

            Debug.Log(totalAnswers);
            Debug.Log(correctAnswersCount);
            Debug.Log((float)correctAnswersCount / (float)totalAnswers);

            if (totalAnswers > 0)
                averageAnswersCount = (float)correctAnswersCount / (float)totalAnswers * 100;
        }

        averageValuesDisplay.text = $"{correctAnswersCount}\n\n{(int)averageAnswersCount}%";
    }
}

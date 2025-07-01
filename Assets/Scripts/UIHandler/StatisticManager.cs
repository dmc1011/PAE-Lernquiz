using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System;
using Entities;

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
    [SerializeField] private GameObject lineGraph;
    [SerializeField] private GameObject timeStatistic;
    [SerializeField] private GameObject averageAnswers;

    public static bool isDailyTaskStatistic;

    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private DailyTaskHistoryTable dailyTaskHistoryTable;
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
    private string formattedTotalDuration = "";
    private string formattedCurrentRunDuration = "";
    private string formattedAverageRunDuration = "";

    // current session
    int isCorrectCount = 0;
    int isFalseCount = 0;
    int notAnsweredCount;

    // Start is called before the first frame update
    void Start()
    {
        if (isDailyTaskStatistic)
        {
            dailyTaskHistoryTable = SQLiteSetup.Instance.dailyTaskHistoryTable;

            catalogueStatisticPanel.gameObject.SetActive(false);
            dailyTaskStatisticPanel.gameObject.SetActive(true);
            
            SetDaylyTaskStatistics();
        }
        else 
        {
            currentCatalogue = Global.CurrentQuestionRound.catalogue;
            catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
            answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;
            catalogueTable = SQLiteSetup.Instance.catalogueTable;

            notAnsweredCount = currentCatalogue.questions.Count;

            dailyTaskStatisticPanel.gameObject.SetActive(false);
            catalogueName.transform.SetParent(scrollTransform);
            barProgress.transform.SetParent(scrollTransform);
            averageAnswers.transform.SetParent(scrollTransform);
            timeStatistic.transform.SetParent(scrollTransform);
            catalogueNameDisplay.text = Global.CurrentQuestionRound.catalogue.name;
            catalogueLevelDisplay.text = "Quiz-Runde: " + Global.CurrentQuestionRound.catalogue.sessionCount.ToString();

            catalogueSessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(currentCatalogue.id);
            SetStatistics();
        }
    }

    private void SetDaylyTaskStatistics()
    {
        lineGraph.GetComponent<LineGraph>().SetGraph(GetFormattedDaily(), "Antworten", 0, 5, 0, 10);
    }

    private List<string> GetFormattedDaily()
    {
        List<DailyTaskHistory> dailyTaskHistory = dailyTaskHistoryTable.FindAllDailyTaskEntries();
        DateTime now = DateTime.Now;

        switch (dailyTaskHistory.Count)
        {
            case 0:
                return new List<string> {$"{now:dd.MM},0",$"{now.AddDays(-1):dd.MM},0",$"{now.AddDays(-2):dd.MM},0",$"{now.AddDays(-3):dd.MM},0",$"{now.AddDays(-4):dd.MM},0"};
            default:
                return new List<string> {$"{dailyTaskHistory[0].taskDate:dd.MM},{dailyTaskHistory[0].correctAnswersCount}",
                                        $"{dailyTaskHistory[1].taskDate:dd.MM},{dailyTaskHistory[1].correctAnswersCount}",
                                        $"{dailyTaskHistory[2].taskDate:dd.MM},{dailyTaskHistory[2].correctAnswersCount}",
                                        $"{dailyTaskHistory[3].taskDate:dd.MM},{dailyTaskHistory[3].correctAnswersCount}",
                                        $"{dailyTaskHistory[4].taskDate:dd.MM},{dailyTaskHistory[4].correctAnswersCount}"};
            case 4:
                return new List<string> {$"{dailyTaskHistory[0].taskDate:dd.MM},{dailyTaskHistory[0].correctAnswersCount}",
                                        $"{dailyTaskHistory[1].taskDate:dd.MM},{dailyTaskHistory[1].correctAnswersCount}",
                                        $"{dailyTaskHistory[2].taskDate:dd.MM},{dailyTaskHistory[2].correctAnswersCount}",
                                        $"{dailyTaskHistory[3].taskDate:dd.MM},{dailyTaskHistory[3].correctAnswersCount}",
                                        $"{now.AddDays(-4):dd.MM},0"};
            case 3:
                return new List<string> {$"{dailyTaskHistory[0].taskDate:dd.MM},{dailyTaskHistory[0].correctAnswersCount}",
                                        $"{dailyTaskHistory[1].taskDate:dd.MM},{dailyTaskHistory[1].correctAnswersCount}",
                                        $"{dailyTaskHistory[2].taskDate:dd.MM},{dailyTaskHistory[2].correctAnswersCount}",
                                        $"{now.AddDays(-3):dd.MM},0",
                                        $"{now.AddDays(-4):dd.MM},0"};
            case 2:
                return new List<string> {$"{dailyTaskHistory[0].taskDate:dd.MM},{dailyTaskHistory[0].correctAnswersCount}",
                                        $"{dailyTaskHistory[1].taskDate:dd.MM},{dailyTaskHistory[1].correctAnswersCount}",
                                        $"{now.AddDays(-2):dd.MM},0",
                                        $"{now.AddDays(-3):dd.MM},0",
                                        $"{now.AddDays(-4):dd.MM},0"};
            case 1:
                return new List<string> {$"{dailyTaskHistory[0].taskDate:dd.MM},{dailyTaskHistory[0].correctAnswersCount}",
                                        $"{now.AddDays(-1):dd.MM},0",
                                        $"{now.AddDays(-2):dd.MM},0",
                                        $"{now.AddDays(-3):dd.MM},0",
                                        $"{now.AddDays(-4):dd.MM},0"};
        }
    }

    private string GetFormattedTimeInHMS(int timeInSeconds)
    {
        int hours = timeInSeconds / 3600;
        int minutes = (timeInSeconds % 3600) / 60;
        int seconds = timeInSeconds % 60;

        return $"{hours}h {minutes}min {seconds}s";
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

            int totalDurationSum = catalogueSessionHistories.Sum(history => history.timeSpent);
            averageRunDuration = totalDurationSum / catalogueSessionHistories.Count;
        }

        totalDuration = currentCatalogue.totalTimeSpent;

        formattedTotalDuration = GetFormattedTimeInHMS(currentCatalogue.totalTimeSpent);
        formattedCurrentRunDuration = GetFormattedTimeInHMS(currentRunDuration);
        formattedAverageRunDuration = GetFormattedTimeInHMS(averageRunDuration);

        timeValuesDisplay.text = $"{formattedCurrentRunDuration}\n\n{formattedAverageRunDuration}\n\n{formattedTotalDuration}";
    }

    private void AverageCorrectAnswersInCatalogue()
    {
        int totalAnswers = 0;

        List<Question> questions = catalogueTable.FindQuestionsByCatalogueId(currentCatalogue.id);

        foreach (var question in questions)
        {
            correctAnswersCount += question.correctAnsweredCount;
            totalAnswers += question.totalAnsweredCount;

            if (totalAnswers > 0)
                averageAnswersCount = (float)correctAnswersCount / (float)totalAnswers * 100;
        }

        averageValuesDisplay.text = $"{correctAnswersCount}\n\n{(int)averageAnswersCount}%";
    }
}
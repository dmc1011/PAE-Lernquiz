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
    [SerializeField] private BarConfig barConfig;

    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private AnswerHistoryTable answerHistoryTable;
    private List<CatalogueSessionHistory> catalogueSessionHistories;
    private Catalogue currentCatalogue;
    private int currentRunDuration = 0;
    private int averageRunDuration = 0;
    private int totalDuration = 0;
    private string totalDurationUnit = "";
    private string currentRunDurationUnit = "";
    private string averageRunDurationUnit = "";
    public static bool isDailyTaskStatistic;

    // Start is called before the first frame update
    void Start()
    {
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
        answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;

        if (isDailyTaskStatistic)
        {
            catalogueStatisticPanel.gameObject.SetActive(false);
            dailyTaskStatisticPanel.gameObject.SetActive(true);
        }
        else 
        {
            dailyTaskStatisticPanel.gameObject.SetActive(false);
            catalogueNameDisplay.text = Global.CurrentQuestionRound.catalogue.name;
            catalogueLevelDisplay.text = "Stufe " + Global.CurrentQuestionRound.catalogue.sessionCount.ToString();

            catalogueSessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(currentCatalogue.id);
            SetStatistics();
            //to do: set catalogue level as text
            timeValuesDisplay.text = $"{currentRunDuration}{currentRunDurationUnit}\n\n{averageRunDuration}{averageRunDurationUnit}\n\n{totalDuration}{totalDurationUnit}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string GetUnit(int seconds)
    {
        return seconds > 60 ? " Minuten" : " Sekunden";
    }

    private void SetStatistics()
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

        int isCorrectCount = 0;
        int isFalseCount = 0;
        int notAnsweredCount = currentCatalogue.questions.Count;
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
}

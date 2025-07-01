using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Entities;

public class NewGameManager : MonoBehaviour
{
    [SerializeField] private GameObject catalogueButtonPrefab;      // used for dynamically rendering catalogue buttons
    [SerializeField] private GameObject dailyTaskButtonPrefab;
    [SerializeField] private GameObject addButton;
    [SerializeField] private Transform buttonContainer;             // 'content' element of scroll view
    [SerializeField] private HexagonBackground bg = null;

    [HideInInspector] public static CatalogueTable catalogueTable;
    [HideInInspector] public static int catalogueCount;
    [HideInInspector] public static List<Catalogue> catalogues;
    [HideInInspector] public static GameMode gameMode;

    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private AnswerHistoryTable answerHistoryTable;

    void Start()
    {
        // Failsafe
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any other scene than 'NewGame'.");
            return;
        }

        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
        answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;
        gameMode = Global.CurrentQuestionRound.gameMode;       // get current GameMode: defines behavior of events triggered by selecting a catalogue

        SetCatalogueButtons();
    }

    // renders a catalogue button for each existing catalogue on the scroll view element
    private void SetCatalogueButtons()
    {
        if (Global.CurrentQuestionRound.gameMode == GameMode.Statistics)
        {
            GameObject catalogueButton = Instantiate(dailyTaskButtonPrefab, buttonContainer);

            // set background on runtime
            var manager = catalogueButton.GetComponent<CatalogueButtonHandler>();
            manager.SetBackground(bg);

            TextMeshProUGUI buttonLabel = catalogueButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = "Daily Task";
        }

        if (Global.CurrentQuestionRound.gameMode == GameMode.Editor)
        {
            addButton.gameObject.SetActive(true);
        }

        for (int i = 0; i < catalogueCount; i++)
        {
            if (Global.CurrentQuestionRound.gameMode == GameMode.PracticeBook)
            {
                List<Question> questions = catalogues[i].questions;
                bool catalogueContainsPracticeQuestions = questions.Any(questions => questions.enabledForPractice);
                if (!catalogueContainsPracticeQuestions)
                    continue;
            }
            GameObject catalogueButton = Instantiate(catalogueButtonPrefab, buttonContainer);
            SliderConfig sliderConfig = catalogueButton.GetComponentInChildren<SliderConfig>();

            // set background on runtime
            var manager = catalogueButton.GetComponent<CatalogueButtonHandler>();
            manager.SetBackground(bg);

            // display catalogue name on button
            TextMeshProUGUI buttonLabel = catalogueButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = catalogues[i].name;

            SetProgressCircleStatistics(catalogues[i], sliderConfig);
        }
    }

    private void SetProgressCircleStatistics(Catalogue catalogue, SliderConfig sliderConfig)
    {
        int isCorrectCount = 0;
        int isFalseCount = 0;
        int notAnsweredCount = catalogue.questions.Count;
        CatalogueSessionHistory session = catalogueSessionHistoryTable.FindLatestCatalogueSessionHistoryByCatalogueId(catalogue.id);
        if (session != null)
        {
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
       
        sliderConfig.SetValue(isCorrectCount, isFalseCount, notAnsweredCount);
        sliderConfig.SetLabel(catalogue.sessionCount.ToString());
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationManager : MonoBehaviour
{

    [SerializeField] private EvaluationButton evaluationButtonPrefab;
    [SerializeField] private GameObject NewGameButton;
    [SerializeField] private EvaluationStatistics evaluationStatistics;
    [SerializeField] private Transform scrollTransform;
    [SerializeField] private SideMenu sideMenu;
    [SerializeField] private QuizAreaManager quizAreaManager;
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private GameObject newLevelPopup;
    [SerializeField] private TextMeshProUGUI popupMessage;
    [SerializeField] private TextMeshProUGUI xpInfo;

    private List<EvaluationButton> evaluationButtons = new();
    private List<DataManager.QuestionResult> questionResults = new();
    //private string evaluationFor;
    private GameMode gameMode;
    private DailyTaskHistoryTable dailyTaskHistoryTable;

    void Start()
    {
        //evaluationFor = PlayerPrefs.GetString("evaluationFor");
        gameMode = Global.CurrentQuestionRound.gameMode;
        Debug.Log(gameMode);
        dailyTaskHistoryTable = SQLiteSetup.Instance.dailyTaskHistoryTable;

        switch (gameMode) {
            case GameMode.DailyTask:
                NewGameButton.SetActive(false);
                dailyTaskHistoryTable.SetTodaysTaskToCompleted();
                PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "true");
                PlayerPrefs.Save();
                LoadResults(GetDailyTaskResults());
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Daylies");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Besserwisser");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Hartn‰ckig");
                break;
            case GameMode.LinearQuiz:
                LoadResults(DataManager.QuestionResults);
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Fleiﬂig");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Fokus", Global.CurrentQuestionRound.catalogue);
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Zeitmanagement");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Intensiv");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Flawless", Global.CurrentQuestionRound.catalogue);
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Multitalent");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Besserwisser");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Hartn‰ckig");
                break;
            case GameMode.RandomQuiz:
            case GameMode.PracticeBook:
                LoadResults(DataManager.QuestionResults);
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Randomat");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Random Flawless");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Besserwisser");
                achievementManager.CheckNextAchievementLevelAndSetAchievedStatus("Hartn‰ckig");
                break;
            default:
                print("ERROR: EvaluationManager - Invalid Game Mode for evaluation.");
                break;
        }

        if (gameMode == GameMode.DailyTask && PlayerLevel.gainedXp == 0)
        {
            xpInfo.text = "";
        } 
        else
        {
            xpInfo.text = $"Du hast {PlayerLevel.gainedXp} XP verdient.";
        }

        PlayerLevel.AddPlayerXp(newLevelPopup, popupMessage);
    }

    private List<DataManager.QuestionResult> GetDailyTaskResults()
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
                answerTexts = allAnswerTexts,
                questionId = PlayerPrefs.GetInt($"dailyQuestionId{count}")
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

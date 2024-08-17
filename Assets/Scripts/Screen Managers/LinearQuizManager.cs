using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinearQuizManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private QuizAreaManager quizAreaManager;
    [SerializeField] private Button nextButton;
    [SerializeField] private ButtonNavigation nextButtonNavigation;

    private Catalogue currentCatalogue;
    private List<Question> questions;
    private int nextQuestionIndex = 0;
    private CatalogueTable catalogueTable;
    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private AnswerHistoryTable answerHistoryTable;
    private DateTime startTime;
    private DateTime subSessionStartTime;
    private int currentSessionId;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "LinearQuiz");
        PlayerPrefs.Save();

        DataManager.ClearResults();
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
        answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;

        // Get current catalogue
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        questions = currentCatalogue.questions;
        nextButton.interactable = false;
        startTime = DateTime.Now;
        subSessionStartTime = DateTime.Now;

        SetEntryPoint();

        // Display the first question
        DisplayNextQuestion();
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        if (nextQuestionIndex >= questions.Count)
        {
            nextQuestionIndex = 0;

            UpdateSessionHistory();
            int newSessionId = catalogueSessionHistoryTable.AddCatalogueSessionHistory(currentCatalogue.id, 0, false);
            currentSessionId = newSessionId;
            subSessionStartTime = DateTime.Now;
        }

        Question nextQuestion = questions[nextQuestionIndex];
        currentCatalogue.currentQuestionId = nextQuestion.id;
        catalogueTable.UpdateCatalogue(currentCatalogue);

        quizAreaManager.ResetContents();
        quizAreaManager.RandomizePositions();
        quizAreaManager.SetContents(nextQuestion);

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {nextQuestionIndex + 1}";
        nextButton.interactable = false;
        nextQuestionIndex += 1;
    }

    public void EventButtonPressedCallback(QuizAreaManager.ButtonID button)
    {
        switch (button)
        {
            case QuizAreaManager.ButtonID.Q:
                {
                    // MS: There is currently no logic involved in pressing the question button.
                    // But the event is forwarded for potential later use.
                }
                break;

            case QuizAreaManager.ButtonID.A: // MS: I wanted to write it "exactly this way" to support
            case QuizAreaManager.ButtonID.B: // the case where we have different logic for different buttons.
            case QuizAreaManager.ButtonID.C: // Currently it's all the same. I know.
            case QuizAreaManager.ButtonID.D: // This also filters any unwanted values of "button" if we add something in the future.
                {
                    int questionIndex = nextQuestionIndex - 1;
                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);
                    nextButton.interactable = true;
                }
                break;
        }

    }

    public void LoadNextScene()
    {
        nextButtonNavigation.LoadScene("Evaluation");
    }

    private void OnApplicationQuit()
    {
        UpdateSessionHistory();
        SaveTimeSpent();
    }

    public void SaveTimeSpent()
    {
        TimeSpan duration = DateTime.Now - startTime;
        int secondsSpent = (int)duration.TotalSeconds;

        // Update TotalTimeSpent in the current catalogue
        currentCatalogue.totalTimeSpent += secondsSpent;
        catalogueTable.UpdateCatalogue(currentCatalogue);

        UpdateSessionHistory();
    }

    private void UpdateSessionHistory()
    {
        TimeSpan duration = DateTime.Now - subSessionStartTime;
        int secondsSpent = (int)duration.TotalSeconds;

        CatalogueSessionHistory currentSessionHistory = catalogueSessionHistoryTable.FindCatalogueSessionHistoryById(currentSessionId);
        List<AnswerHistory> answerHistoriesForSession = answerHistoryTable.FindAnswerHistoryBySessionId(currentSessionId);
        bool sessionCompleted = answerHistoriesForSession.Count == currentCatalogue.questions.Count;

        if (sessionCompleted)
        {
            currentCatalogue.sessionCount = currentCatalogue.sessionCount + 1;
            catalogueTable.UpdateCatalogue(currentCatalogue);
        }

        catalogueSessionHistoryTable.UpdateCatalogueSessionHistory(currentSessionId, currentSessionHistory.timeSpent + secondsSpent, sessionCompleted);
    }

    private void SetEntryPoint()
    {

        int currentQuestionIndex = currentCatalogue.questions.FindIndex(q => q.id == currentCatalogue.currentQuestionId);
        if (currentQuestionIndex != -1)
            nextQuestionIndex = currentQuestionIndex;

        if (nextQuestionIndex == 0)
        {
            // This avoids the creation of a new session for the case that the user stops at question 0 and then starts again
            CatalogueSessionHistory currentSession = catalogueSessionHistoryTable.FindLatestCatalogueSessionHistoryByCatalogueId(currentCatalogue.id);
            currentSessionId = (currentSession == null || currentSession.isCompleted) ? catalogueSessionHistoryTable.AddCatalogueSessionHistory(currentCatalogue.id, 0, false) : currentSession.id;
        }
        else
        {
            CatalogueSessionHistory currentSession = catalogueSessionHistoryTable.FindLatestCatalogueSessionHistoryByCatalogueId(currentCatalogue.id);
            currentSessionId = currentSession.id;
        }
    }
}


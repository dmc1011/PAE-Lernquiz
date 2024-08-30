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
    private int currentSessionId;
    private bool sessionIsErrorFree;
    private TextMeshProUGUI nextButtonLabel;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "LinearQuiz");
        PlayerPrefs.Save();

        DataManager.ClearResults();
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
        answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;

        nextButtonLabel = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get current catalogue
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        questions = currentCatalogue.questions;
        nextButton.interactable = false;
        startTime = DateTime.Now;

        SetEntryPoint();

        // Display the first question
        DisplayNextQuestion();
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        if (nextQuestionIndex >= questions.Count)
        {
            SaveTimeSpent();
            LoadNextScene();
            return;
        }

        Question nextQuestion = questions[nextQuestionIndex];

        quizAreaManager.ResetContents();
        quizAreaManager.RandomizePositions();
        quizAreaManager.SetContents(nextQuestion);

        if (nextQuestionIndex == questions.Count - 1)
            nextButtonLabel.text = "Beenden";

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

                    if (nextQuestionIndex < questions.Count)
                    {
                        Question nextQuestion = questions[nextQuestionIndex];
                        currentCatalogue.currentQuestionId = nextQuestion.id;
                        catalogueTable.UpdateCatalogue(currentCatalogue);
                    }

                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);
                    sessionIsErrorFree = sessionIsErrorFree && (int)button == 0;
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
        TimeSpan duration = DateTime.Now - startTime;
        int secondsSpent = (int)duration.TotalSeconds;

        CatalogueSessionHistory currentSessionHistory = catalogueSessionHistoryTable.FindCatalogueSessionHistoryById(currentSessionId);
        List<AnswerHistory> answerHistoriesForSession = answerHistoryTable.FindAnswerHistoryBySessionId(currentSessionId);
        bool sessionCompleted = answerHistoriesForSession.Count >= currentCatalogue.questions.Count;

        if (sessionCompleted)
        {
            currentCatalogue.sessionCount++;
            // invalid question id --> field is set to null in db
            currentCatalogue.currentQuestionId = -1;
            if (sessionIsErrorFree)
                currentCatalogue.errorFreeSessionCount++;

            catalogueTable.UpdateCatalogue(currentCatalogue);
        }

        catalogueSessionHistoryTable.UpdateCatalogueSessionHistory(currentSessionId, currentSessionHistory.timeSpent + secondsSpent, sessionCompleted, sessionIsErrorFree);
    }

    private void SetEntryPoint()
    {

        int currentQuestionIndex = questions.FindIndex(q => q.id == currentCatalogue.currentQuestionId);
        if (currentQuestionIndex != -1)
            nextQuestionIndex = currentQuestionIndex;

        CatalogueSessionHistory currentSession = catalogueSessionHistoryTable.FindLatestCatalogueSessionHistoryByCatalogueId(currentCatalogue.id);
        if (nextQuestionIndex == 0)
        {
            // This avoids the creation of a new session for the case that the user stops at question 0 and then starts again
            currentSessionId = (currentSession == null || currentSession.isCompleted) ? catalogueSessionHistoryTable.AddCatalogueSessionHistory(currentCatalogue.id, 0, false) : currentSession.id;
            sessionIsErrorFree = (currentSession == null || currentSession.isCompleted) ? true : currentSession.isErrorFree;
        }
        else
        {
            currentSessionId = currentSession.id;
            sessionIsErrorFree = currentSession.isErrorFree;
        }
    }
}


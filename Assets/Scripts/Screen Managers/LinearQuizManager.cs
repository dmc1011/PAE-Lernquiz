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
    private DateTime startTime;
    private DateTime sessionStartTime;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "LinearQuiz");
        PlayerPrefs.Save();

        DataManager.ClearResults();
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;

        // Get current catalogue
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        questions = currentCatalogue.questions;
        nextButton.interactable = false;
        startTime = DateTime.Now;
        sessionStartTime = DateTime.Now;

        int currentQuestionIndex = currentCatalogue.questions.FindIndex(q => q.id == currentCatalogue.currentQuestionId);
        if (currentQuestionIndex != -1)
            nextQuestionIndex = currentQuestionIndex;

        // Display the first question
        DisplayNextQuestion();
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        if (nextQuestionIndex >= questions.Count)
        {
            nextQuestionIndex = 0;
            TimeSpan duration = DateTime.Now - sessionStartTime;
            int secondsSpent = (int)duration.TotalSeconds;

            catalogueSessionHistoryTable.AddCatalogueSessionHistory(currentCatalogue.id, secondsSpent);
            sessionStartTime = DateTime.Now;
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
        SaveTimeSpent();
    }

    public void SaveTimeSpent()
    {
        TimeSpan duration = DateTime.Now - startTime;
        int secondsSpent = (int)duration.TotalSeconds;

        // Update TotalTimeSpent in the current catalogue
        currentCatalogue.totalTimeSpent += secondsSpent;
        catalogueTable.UpdateCatalogue(currentCatalogue);
    }
}


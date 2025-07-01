using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class QuizHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private QuizAreaManager quizAreaManager;
    [SerializeField] private Button nextButton;
    [SerializeField] private ButtonNavigation nextButtonNavigation;

    private Catalogue currentCatalogue;
    private List<Question> questions;

    private DateTime startTime;
    private int nextQuestionIndex = 0;
    private bool sessionIsErrorFree;
    private int currentSessionId;
    private bool questionHasBeenAnswered;



    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefsManager.SetEvaluationType("LinearQuiz");

        DataManager.ClearResults();

        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        questions = currentCatalogue.questions;
        nextButton.interactable = false;
        startTime = DateTime.Now;

        SetEntryPoint();

        DisplayNextQuestion();
    }

    private void SetEntryPoint()
    {

        int currentQuestionIndex = questions.FindIndex(q => q.id == currentCatalogue.currentQuestionId);
        if (currentQuestionIndex != -1)
        {
            nextQuestionIndex = currentQuestionIndex;
        }

        CatalogueSessionHistory currentSession = Global.GetCatalogue().sessionHistories.Count > 0 ? Global.GetCatalogue().sessionHistories.Last() : null;

        // if no session exists or current session is completed start new session
        if (nextQuestionIndex == 0 && (currentSession == null || currentSession.isCompleted))
        {
            currentSessionId = currentSession == null ? 0 : currentSession.id + 1;
            sessionIsErrorFree = true;
            currentSession = new CatalogueSessionHistory(currentSessionId, currentCatalogue.id, DateTime.Now, 0, false, sessionIsErrorFree);
            currentCatalogue.sessionHistories.Add(currentSession);
            currentCatalogue.sessionCount++;
        }
        else
        {
            currentSessionId = currentSession.id;
            sessionIsErrorFree = currentSession.isErrorFree;
        }

        Global.SetCatalogue(currentCatalogue);   
    }

    public void DisplayNextQuestion()
    {
        questionHasBeenAnswered = false;

        Question nextQuestion = questions[nextQuestionIndex];

        if (questions[nextQuestionIndex].id == questions.Last().id)
        {
            nextButton.gameObject.SetActive(false);
        }
        else 
        {
            nextButton.gameObject.SetActive(true);
        }

        quizAreaManager.DisplayNextQuestion(nextQuestion);

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {nextQuestionIndex + 1}";
        nextButton.interactable = false;
        nextQuestionIndex += 1;
    }

    public void EventButtonPressedCallback(QuizAreaManager.ButtonID button)
    {
        questionHasBeenAnswered = true;

        switch (button)
        {
            case QuizAreaManager.ButtonID.Q:
                break;

            case QuizAreaManager.ButtonID.A:
            case QuizAreaManager.ButtonID.B:
            case QuizAreaManager.ButtonID.C:
            case QuizAreaManager.ButtonID.D:
                {
                    int questionIndex = nextQuestionIndex - 1;

                    if (nextQuestionIndex < questions.Count)
                    {
                        Question nextQuestion = questions[nextQuestionIndex];
                        currentCatalogue.currentQuestionId = nextQuestion.id;
                        Global.SetCatalogue(currentCatalogue);
                    }

                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);

                    sessionIsErrorFree = sessionIsErrorFree && (int)button == 0;

                    if (nextQuestionIndex <= questions.Count)
                        nextButton.interactable = true;
                }
                break;
        }

    }

    public void LoadEvaluation()
    {
        nextButtonNavigation.LoadScene(Scene.Evaluation);
    }

    private void OnApplicationQuit()
    {
        SaveTimeSpent();
    }

    public void SaveTimeSpent()
    {
        TimeSpan duration = DateTime.Now - startTime;
        int secondsSpent = (int)duration.TotalSeconds;

        currentCatalogue.totalTimeSpent += secondsSpent;
        Global.SetCatalogue(currentCatalogue);

        UpdateSessionHistory(secondsSpent);
    }

    private void UpdateSessionHistory(int timeSpent)
    {
        Catalogue catalogue = Global.GetCatalogue();
        CatalogueSessionHistory currentSessionHistory = catalogue.sessionHistories.Last();
        List<AnswerHistory> answerHistoriesForSession = Global.GetAnswerHistories();

        bool sessionCompleted = currentCatalogue.currentQuestionId == questions.Last().id && questionHasBeenAnswered;

        // to do: compare with .UpdateCatalogue, replace .UpdateCatalogueHistory
        if (sessionCompleted)
        {
            // invalid question id -> field is set to null in db
            currentCatalogue.currentQuestionId = questions.First().id;
            if (sessionIsErrorFree)
                currentCatalogue.errorFreeSessionCount++;
        }

        currentSessionHistory.timeSpent += timeSpent;
        currentSessionHistory.isCompleted = sessionCompleted;
        currentSessionHistory.isErrorFree = sessionIsErrorFree;

        int index = catalogue.sessionHistories.FindIndex(session => session.id == currentSessionId);

        if (index != -1)
        {
            currentCatalogue.sessionHistories[index] = currentSessionHistory;
        }

        Global.SetCatalogue(currentCatalogue);
        Global.UpdateCatalogueUserData();
    }
}

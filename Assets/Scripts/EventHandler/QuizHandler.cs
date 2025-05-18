using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using TMPro;
using UnityEngine.UI;

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



    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefsManager.SetEvaluationType("LinearQuiz");

        DataManager.ClearResults();

        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        questions = currentCatalogue.questions;
        nextButton.interactable = false;
        startTime = DateTime.Now;

        // to do: Global.InitializeUserSessionHistory() -> new list with 1 empty session

        SetEntryPoint();

        DisplayNextQuestion();
    }

    public void DisplayNextQuestion()
    {
        Question nextQuestion = questions[nextQuestionIndex];

        quizAreaManager.DisplayNextQuestion(nextQuestion);

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {nextQuestionIndex + 1}";
        nextButton.interactable = false;
        nextQuestionIndex += 1;
    }

    private void SetEntryPoint()
    {

        int currentQuestionIndex = questions.FindIndex(q => q.id == currentCatalogue.currentQuestionId);
        if (currentQuestionIndex != -1)
        {
            nextQuestionIndex = currentQuestionIndex;
        }

        /* to do: only track new data, handle merge in evaluation

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
        */
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
                    }

                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);

                    // to do: sessionIsErrorFree = sessionIsErrorFree && (int)button == 0;

                    if (nextQuestionIndex != questions.Count)
                        nextButton.interactable = true;
                }
                break;
        }

    }
}

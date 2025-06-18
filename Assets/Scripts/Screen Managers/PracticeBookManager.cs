using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Entities;

public class PracticeBookManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private Button nextButton;
    [SerializeField] private ButtonNavigation nextButtonNavigation;
    [SerializeField] private GameObject questionSelectionScrollView;
    [SerializeField] private GameObject quizAreaContainer;
    [SerializeField] private GameObject questionButtonPrefab;      // used for dynamically rendering question buttons
    [SerializeField] private Transform buttonContainer;            // 'content' element of scroll view
    [SerializeField] private HexagonBackground background;

    private Catalogue currentCatalogue;
    private List<Question> questions;
    private List<Question> allQuestions;
    private int nextQuestionIndex = 0;
    private CatalogueTable catalogueTable;
    private QuizAreaManager quizAreaManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "PracticeBook");
        PlayerPrefs.Save();
        DataManager.ClearResults();

        // Get current catalogue and flagged questions
        // catalogueTable = SQLiteSetup.Instance.catalogueTable;
        currentCatalogue = Global.GetCatalogue();
        allQuestions = currentCatalogue.questions;
        questions = allQuestions.Where(q => q.enabledForPractice).ToList();

        quizAreaContainer.SetActive(false);
        quizAreaManager = quizAreaContainer.GetComponentInChildren<QuizAreaManager>();

        DisplayQuestionSelection();
    }

    private void DisplayQuestionSelection()
    {
        if (buttonContainer.transform.childCount > 1)
        {
            for (int i = buttonContainer.transform.childCount - 1; i > 0; i--)
            {
                Destroy(buttonContainer.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < questions.Count; i++)
        {
            GameObject questionButton = Instantiate(questionButtonPrefab, buttonContainer);
            questionButton.SetActive(true);

            // display question name on button
            TextMeshProUGUI buttonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = questions[i].name;
            if (buttonLabel.text == "" || buttonLabel.text == null)
            {
                buttonLabel.text = "Frage " + allQuestions.FindIndex(q => q == questions[i]) + 1;
            }
        }

        questionSelectionScrollView.gameObject.SetActive(true);
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        Question nextQuestion = questions[nextQuestionIndex];

        quizAreaManager.DisplayNextQuestion(nextQuestion);

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {allQuestions.FindIndex(q => q == nextQuestion) + 1}";
        nextButton.interactable = false;
        nextQuestionIndex += 1;
    }

    public void EventButtonPressedCallback(QuizAreaManager.ButtonID button)
    {
        switch (button)
        {
            case QuizAreaManager.ButtonID.Q:
                break;

            case QuizAreaManager.ButtonID.A:
            case QuizAreaManager.ButtonID.B:
            case QuizAreaManager.ButtonID.C:
            case QuizAreaManager.ButtonID.D:
                {
                    int questionIndex = allQuestions.FindIndex(q => q == questions[nextQuestionIndex - 1]);

                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);

                    if (nextQuestionIndex <= questions.Count)
                        nextButton.interactable = true;
                }
                break;
        }

    }

    // Ensures that QuizAreaManagers Start() method is fully executed before calling any other methods
    private IEnumerator WaitAndDisplayFirstQuestion(TextMeshProUGUI questionButtonLabel)
    {
        nextButton.interactable = false;
        questionSelectionScrollView.SetActive(false);
        quizAreaContainer.SetActive(true);

        Question selectedQuestion = currentCatalogue.questions.Find(question => question.name == questionButtonLabel.text); // to do: requires every question to have a unique name
        if (selectedQuestion != null)
        {
            nextQuestionIndex = questions.FindIndex(q => q == selectedQuestion);
            nextQuestionIndex = nextQuestionIndex == -1 ? 0 : nextQuestionIndex;
        }
        yield return null;

        DisplayNextQuestion();
    }

    public void StartQuiz(TextMeshProUGUI questionButtonLabel) 
    {
        StartCoroutine(WaitAndDisplayFirstQuestion(questionButtonLabel));
    }

    public void LoadNextScene()
    {
        nextButtonNavigation.LoadScene(Scene.Evaluation);
    }

    public void LoadCatalogueSelection()
    {
        if (background != null)
        {
            float timeNeeded = background.TriggerEndSequence();
            Invoke(nameof(LoadSceneInternal), timeNeeded);
        }
        else
        {
            LoadSceneInternal();
        }
    }


    private void LoadSceneInternal()
    {
        SceneManager.LoadScene("ContentSelection");
    }
}


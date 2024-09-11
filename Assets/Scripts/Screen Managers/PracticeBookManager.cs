using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        allQuestions = currentCatalogue.questions;
        questions = allQuestions.Where(q => q.enabledForPractice).ToList();

        quizAreaContainer.SetActive(false);
        quizAreaManager = quizAreaContainer.GetComponentInChildren<QuizAreaManager>();

        DisplayQuestionSelection();
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        Question nextQuestion = questions[nextQuestionIndex];
        quizAreaManager.ResetContents();
        quizAreaManager.RandomizePositions();
        quizAreaManager.SetContents(nextQuestion);

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {allQuestions.FindIndex(q => q == nextQuestion) + 1}";
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
                    int questionIndex = allQuestions.FindIndex(q => q == questions[nextQuestionIndex - 1]);
                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);

                    if (nextQuestionIndex != questions.Count)
                        nextButton.interactable = true;
                }
                break;
        }

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
                buttonLabel.text = "Diese Frage hat noch keinen Namen. Das Quiz startet automatisch bei Frage 1";
            }
        }

        questionSelectionScrollView.gameObject.SetActive(true);
    }


    // Ensures that QuizAreaManagers Start() method is fully executed before calling any other methods
    private IEnumerator WaitAndDisplayFirstQuestion(TextMeshProUGUI questionButtonLabel)
    {
        nextButton.interactable = false;
        questionSelectionScrollView.SetActive(false);
        quizAreaContainer.SetActive(true);

        Question selectedQuestion = currentCatalogue.questions.Find(question => question.name == questionButtonLabel.text);
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
        nextButtonNavigation.LoadScene("Evaluation");
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
        SceneManager.LoadScene("NewGame");
    }
}


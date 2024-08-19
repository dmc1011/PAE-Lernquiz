using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Global;


public class RandomQuizManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private QuizAreaManager quizAreaManager;
    [SerializeField] private Button nextButton;
    [SerializeField] private ButtonNavigation nextButtonNavigation;

    private TextMeshProUGUI nextButtonLabel;
    private bool isQuizOver = false;
    private int questionCount = 0;
    private int questionLimit;
    private Catalogue currentCatalogue;
    private int nextQuestionIndex;

    void Start()
    {
        PlayerPrefs.SetString("evaluationFor", "RandomQuiz");
        PlayerPrefs.Save();

        DataManager.ClearResults();

        if (SceneManager.GetActiveScene().name != "RandomQuiz")
        {
            print("ERROR [RandomQuizManager.cs:Start()]: Dont use this script in any scene other than RandomQuiz");
            return;
        }

        // Get components and set default values
        nextButtonLabel = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get current catalogue
        currentCatalogue = Global.CurrentQuestionRound.catalogue;
        questionLimit = Global.RandomQuizSize;
        nextButton.interactable = false;
        
        // Display first question
        DisplayNextQuestion();
    }


    public void DisplayNextQuestion()
    {
        if (isQuizOver || questionCount >= questionLimit)
        {
            LoadNextScene();
            return;
        }
        
        nextQuestionIndex = Global.CurrentQuestionRound.questions[questionCount];
        Question nextQuestion = currentCatalogue.questions[nextQuestionIndex];

        quizAreaManager.ResetContents();
        quizAreaManager.RandomizePositions();
        quizAreaManager.SetContents(nextQuestion);

        // The "last" question is at UBound - 1. The "Beenden" must be shown one question earlier.
        if (questionCount == questionLimit - 1)
            nextButtonLabel.text = "Beenden";

        int questionIndex = CurrentQuestionRound.catalogue.questions.FindIndex(q => q == nextQuestion);
        Fragenummer.text = "Frage " + (questionCount + 1) + "/" + questionLimit + "\n" + currentCatalogue.name + ", " + "Frage " + (questionIndex + 1);
        nextButton.interactable = false;
        questionCount += 1; // questionCount will be 0 when first Question is displayed

        // Quiz will be considered over as soon as last question is displayed
        if (questionCount >= questionLimit)
        {
            isQuizOver = true;
        }

    }

    public void LoadNextScene()
    {
        nextButtonNavigation.LoadScene("Evaluation");
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
                    // in contrast to LinearQuizManager nextQuestionIndex is not update at this point and still valid
                    int questionIndex = nextQuestionIndex;
                    DataManager.AddAnswer(questionIndex, (int)button, currentCatalogue);
                    nextButton.interactable = true;
                }
                break;
        }

    }

}

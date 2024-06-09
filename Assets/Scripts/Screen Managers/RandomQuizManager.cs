using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RandomQuizManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private Button nextButton;

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();
    private List<RectTransform> answerButtonTransforms = new List<RectTransform>();
    private TextMeshProUGUI nextButtonLabel;

    private JsonDataService DataService = new JsonDataService();
    private int selected_answer = 0;
    private bool isQuizOver = false;
    private int questionCount = 0;
    private int questionLimit;
    private ColorBlock defaultColorBlock;
    private Catalogue currentCatalogue;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Gameloop") // TO DO: Gameloop-Szene umbenennen in RandomQuiz
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than Gameloop");
            return;
        }

        // Get components and set default values
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();
        nextButtonLabel = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        foreach (Button button in answerButtons)
        {
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());
            answerButtonTransforms.Add(button.transform.GetComponent<RectTransform>());
        }

        currentCatalogue = DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json");
        defaultColorBlock = answerButtons[0].colors;
        nextButton.interactable = false;
        questionLimit = Global.CurrentQuestionRound.QuestionLimit;

        // Display first question
        DisplayNextQuestion();
    }


    public void DisplayNextQuestion()
    {
        if (isQuizOver)
        {
            LoadNextScene();
            return;
        }
        
        if (questionCount == questionLimit - 1)
        {
            nextButtonLabel.text = "Beenden";
        }

        Question nextQuestion = currentCatalogue.questions[Global.CurrentQuestionRound.Questions[Global.CurrentQuestionRound.QuestionCounter]];
        
        ResetButtons();
        SetRandomizedPositions();
        SetContents(nextQuestion);

        // questionCount will be 0 when first Question is displayed
        questionCount += 1;
        Global.CurrentQuestionRound.QuestionCounter += 1;

        // Quiz will be considered over as soon as last question is displayed
        if (questionCount >= questionLimit)
        {
            isQuizOver = true;
        }
    }


    private void SetRandomizedPositions()
    {
        // Get the current positions
        Vector3[] positions = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            positions[i] = answerButtonTransforms[i].position;
        }

        // Shuffle the positions
        Functions.Shuffle(positions);

        // Apply the new positions
        for (int i = 0; i < 4; i++)
        {
            answerButtonTransforms[i].Translate(positions[i] - answerButtonTransforms[i].position);
        }
    }

    private void SetContents(Question q)
    {
        Debug.Log("The current Catalogue: " + currentCatalogue.name);
        Debug.Log("The current Question: " + q.id);
        questionButtonLabel.text = q.text;

        for (int i = 0; i < 4; i++)
        {
            answerButtonLabels[i].text = q.answers[i].text;
        }

        Fragenummer.text = "Random Quiz, Frage " + (questionCount + 1) + "/" + questionLimit + "\n" + currentCatalogue.name + ", " + "Frage " + q.id; 
    }


    public void LoadNextScene()
    {
        if (isQuizOver)
        {
            SceneManager.LoadScene("Evaluation");
        }
    }


    public void HighlightAnswer(Button button)
    {
        ColorBlock cb = button.colors;
        cb.disabledColor = Color.green;
        answerButtons[0].colors = cb;

        if (button != answerButtons[0])
        {
            cb.disabledColor = Color.red;
            button.colors = cb;
        }

        foreach (Button b in answerButtons)
        {
            b.interactable = false;
        }
        nextButton.interactable = true;
    }


    private void ResetButtons()
    {
        Global.CurrentQuestionRound.ChosenAnswers[Global.CurrentQuestionRound.QuestionCounter] = selected_answer;
        foreach (Button button in answerButtons)
        {
            button.colors = defaultColorBlock;
            button.interactable = true;
        }
        nextButton.interactable = false;
    }
}

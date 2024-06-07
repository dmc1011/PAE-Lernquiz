using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinearQuizManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private Button nextButton;

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();
    private List<RectTransform> answerButtonTransforms = new List<RectTransform>();

    private JsonDataService DataService = new JsonDataService();
    private Catalogue currentCatalogue;
    private int nextQuestionIndex = 0;
    private ColorBlock defaultColorBlock;


    // Start is called before the first frame update
    void Start()
    {
        DataManager.ClearResults();
        // Get components for questionButton
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get components for answer buttons and add them to the lists
        foreach (Button button in answerButtons)
        {
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());
            answerButtonTransforms.Add(button.transform.GetComponent<RectTransform>());
        }

        // Load data and set default values
        currentCatalogue = DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json");
        defaultColorBlock = answerButtons[0].colors;
        nextButton.interactable = false;

        // Display the first question
        DisplayNextQuestion();
    }


    // display question and answer text on the screen
    public void DisplayNextQuestion()
    {
        if (nextQuestionIndex >= currentCatalogue.questions.Count)
        {
            nextQuestionIndex = 0;
        }

        Question nextQuestion = currentCatalogue.questions[nextQuestionIndex];

        ResetButtons();
        SetRandomizedPositions();
        SetContents(nextQuestion);

        nextQuestionIndex += 1;
    }

    // randomly reorder the answer buttons
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
        questionButtonLabel.text = q.text;

        for (int i = 0; i < 4; i++)
        {
            answerButtonLabels[i].text = q.answers[i].text;
        }

        Fragenummer.text = $"{currentCatalogue.name}\nFrage {q.id}";
    }

    public void HighlightAnswer(Button button)
    {
        bool isCorrect = button == answerButtons[0];
        string givenAnswer = button.GetComponentInChildren<TextMeshProUGUI>().text;
        string questionText = questionButtonLabel.text;

        DataManager.AddAnswer(questionText, givenAnswer, isCorrect);

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
        foreach (Button button in answerButtons)
        {
            button.colors = defaultColorBlock;
            button.interactable = true;
        }
        nextButton.interactable = false;
    }
}


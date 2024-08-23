using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizAreaManager : MonoBehaviour
{

    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private MonoBehaviour parentScreenManager;
    [SerializeField] private Color correct;
    [SerializeField] private Color wrong;
    [SerializeField] private GameObject bookmarkIcon;
    [SerializeField] private Color bookmarkActiveColor = new Color(255, 222, 6, 255);
    
    private bool isBookmarkSet;
    private TextMeshProUGUI questionButtonLabel;
    private Question question;
    private AnswerHistoryTable answerHistoryTable;
    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private QuestionTable questionTable;
    private DailyTaskHistoryTable dailyTaskHistoryTable;
    private List<TextMeshProUGUI> answerButtonLabels = new();
    private List<RectTransform> answerButtonTransforms = new();
    private List<Image> answerButtonCorrectImages = new();
    private List<Image> answerButtonWrongImages = new();
    private ColorBlock defaultColorBlock;

    // The enum is used so "button objects" can solely stay in the QuizAreaManager.
    // while notifying the outside world about which buttons were pressed is still possible.
    public enum ButtonID { A /* = 0 -> correct answer */, B, C, D, Q, NONE }; 
    private ButtonID currentlyActiveButton = ButtonID.NONE;

    private bool showResultsOnly = false;

    // Start is called before the first frame update
    void Start()
    {
        answerHistoryTable = SQLiteSetup.Instance.answerHistoryTable;
        questionTable = SQLiteSetup.Instance.questionTable;
        catalogueSessionHistoryTable = SQLiteSetup.Instance.catalogueSessionHistoryTable;
        dailyTaskHistoryTable = SQLiteSetup.Instance.dailyTaskHistoryTable;

        // Get components for questionButton
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get components for answer buttons and add them to the lists
        foreach (Button button in answerButtons)
        {
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());
            answerButtonTransforms.Add(button.transform.GetComponent<RectTransform>());
            Image[] images = button.GetComponentsInChildren<Image>();

            for(int i = 0; i < images.Length; i++)
            {
                if (images[i].name == "Correct")
                    answerButtonCorrectImages.Add(images[i]);
                else if (images[i].name == "Wrong")
                    answerButtonWrongImages.Add(images[i]);
            }
        }
        for(int i = 0; i < answerButtonCorrectImages.Count; i++)
        {
            answerButtonCorrectImages[i].color = new(Mathf.Clamp(correct.r * 2, 0, 1), Mathf.Clamp(correct.g * 2, 0, 1), Mathf.Clamp(correct.b * 2, 0, 1));
            answerButtonWrongImages[i].color = new(Mathf.Clamp(wrong.r * 2, 0, 1), Mathf.Clamp(wrong.g * 2, 0, 1), Mathf.Clamp(wrong.b * 2, 0, 1));
            answerButtonCorrectImages[i].gameObject.SetActive(false);
            answerButtonWrongImages[i].gameObject.SetActive(false);
        }

        // Get the default color of a question button
        defaultColorBlock = answerButtons[0].colors;

        isBookmarkSet = false;
    }

    // Set the contents of all buttons
    public void SetContents(Question q)
    {
        questionButtonLabel.text = q.text;
        question = q;
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtonLabels[i].text = q.answers[i].text;
    }

    public void SetContents(DataManager.QuestionResult result)
    {
        showResultsOnly = true;
        ResetContents();
        questionButtonLabel.text = result.questionText;
        int selectedAnswer = 0;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtonLabels[i].text = result.answerTexts[i];
            if (result.answerTexts[i].Equals(result.selectedAnswerText))
            {
                selectedAnswer = i;
            }
        }
        SetColorsAndDisable((ButtonID)selectedAnswer);
    }

    private void SetColorsAndDisable(ButtonID buttonID)
    {
        ColorBlock cb;
        cb = answerButtons[(int)ButtonID.A].colors;
        cb.disabledColor = new(correct.r, correct.g, correct.b);
        answerButtons[(int)ButtonID.A].colors = cb;
        answerButtonCorrectImages[(int)ButtonID.A].gameObject.SetActive(true);
        if (buttonID != ButtonID.A)
        {
            cb = answerButtons[(int)buttonID].colors;
            cb.disabledColor = new(wrong.r, wrong.g, wrong.b);
            answerButtons[(int)buttonID].colors = cb;
            answerButtonWrongImages[(int)buttonID].gameObject.SetActive(true);
        }
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].interactable = false;
        questionButton.interactable = false;
    }

    // Reset the contents of all buttons
    public void ResetContents()
    {
        for(int i = 0; i < answerButtons.Length; i++)
        {
            answerButtonLabels[i].text = "";
            answerButtons[i].colors = defaultColorBlock;
            answerButtons[i].interactable = true;
            answerButtonCorrectImages[i].gameObject.SetActive(false);
            answerButtonWrongImages[i].gameObject.SetActive(false);
        }
        questionButton.interactable = true;
        currentlyActiveButton = ButtonID.NONE;
    }

    // Randomly reorder the answer buttons
    public void RandomizePositions()
    {
        // Get the current positions
        Vector3[] positions = new Vector3[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
            positions[i] = answerButtonTransforms[i].position;

        // Shuffle the positions
        Functions.Shuffle(positions);

        // Apply the new positions
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtonTransforms[i].Translate(positions[i] - answerButtonTransforms[i].position);

    }

    // Handle the events of button presses
    public void EventButtonPressed(Button button)
    {
        // While in evaluation, button presses should do nothing.
        if(showResultsOnly)
            return;

        if(button == questionButton)
            currentlyActiveButton = ButtonID.Q;
        else
            currentlyActiveButton = (ButtonID)Array.IndexOf(answerButtons, button);

        // There is currently no logic involved if someone uses the questionbutton.
        // this might be useful in the future (e.g. for giving hints? or something?) though.
        if (currentlyActiveButton == ButtonID.Q)
            return;

        SetColorsAndDisable(currentlyActiveButton);

        bool wasCorrect = currentlyActiveButton == ButtonID.A;
        if (wasCorrect)
        {
            question.correctAnsweredCount++;
            dailyTaskHistoryTable.IncrementCorrectAnsweredCount();
        }
        question.totalAnsweredCount++;
        questionTable.UpdateQuestion(question);

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.Equals("LinearQuiz"))
        {
            CatalogueSessionHistory currentSession = catalogueSessionHistoryTable.FindLatestCatalogueSessionHistoryByCatalogueId(question.catalogueId);
            answerHistoryTable.AddAnswerHistory(question.id, wasCorrect, currentSession.id);
        }

        // MS: Ich wei� wirklich nicht ob das "der richtige" weg ist wie man in Unity Callbacks
        // veranstaltet... Wenn sich hier jemand auskennt kann er/sie diesen Kommentar entfernen und das �ndern.
        parentScreenManager.BroadcastMessage("EventButtonPressedCallback", currentlyActiveButton);
        
    }

    public void SetBookmarkIcon ()
    {
        Image img = bookmarkIcon.GetComponent<Image>();

            if (!isBookmarkSet) 
            {
                isBookmarkSet = !isBookmarkSet;
                img.color = bookmarkActiveColor;
            } 
            else 
            {
                isBookmarkSet = !isBookmarkSet;
                img.color = Color.white;
            }
    }

    public void SaveBookmark()
    {
        question.enabledForPractice = isBookmarkSet;
        questionTable.UpdateQuestion(question);
    }

}

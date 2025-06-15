using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class QuizAreaManager : MonoBehaviour
{
    public enum ButtonID { A /* = 0 -> correct answer */, B, C, D, Q, NONE };

    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private Color correct;
    [SerializeField] private Color wrong;
    [SerializeField] private GameObject bookmarkIcon;
    [SerializeField] private Color bookmarkActiveColor = new Color(255, 222, 6, 255);
    [SerializeField] private MonoBehaviour parentScreenManager;

    private TextMeshProUGUI questionButtonLabel;
    private Question question;
    private List<TextMeshProUGUI> answerButtonLabels = new();
    private List<RectTransform> answerButtonTransforms = new();
    private List<Image> answerButtonCorrectImages = new();
    private List<Image> answerButtonWrongImages = new();
    private ColorBlock defaultColorBlock;
    private bool isBookmarkSet;
    private ButtonID currentlyActiveButton = ButtonID.NONE;
    private bool showResultsOnly = false;


    private void Awake()
    {
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        foreach (Button button in answerButtons)
        {
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());
            answerButtonTransforms.Add(button.transform.GetComponent<RectTransform>());
            Image[] images = button.GetComponentsInChildren<Image>();

            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].name == "Correct")
                    answerButtonCorrectImages.Add(images[i]);
                else if (images[i].name == "Wrong")
                    answerButtonWrongImages.Add(images[i]);
            }
        }

        for (int i = 0; i < answerButtonCorrectImages.Count; i++)
        {
            answerButtonCorrectImages[i].color = new(Mathf.Clamp(correct.r * 2, 0, 1), Mathf.Clamp(correct.g * 2, 0, 1), Mathf.Clamp(correct.b * 2, 0, 1));
            answerButtonWrongImages[i].color = new(Mathf.Clamp(wrong.r * 2, 0, 1), Mathf.Clamp(wrong.g * 2, 0, 1), Mathf.Clamp(wrong.b * 2, 0, 1));
            answerButtonCorrectImages[i].gameObject.SetActive(false);
            answerButtonWrongImages[i].gameObject.SetActive(false);
        }

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


        isBookmarkSet = q.enabledForPractice;
        SetInitialBookmarkIcon();
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
        {
            answerButtons[i].interactable = false;
        }

        questionButton.interactable = false;
    }


    public void DisplayNextQuestion(Question q)
    {
        ResetContents();
        RandomizePositions();
        SetContents(q);
    }


    // Reset the contents of all buttons
    public void ResetContents()
    {
        for (int i = 0; i < answerButtons.Length; i++)
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
        if (showResultsOnly)
            return;

        // determine pressed button
        if (button == questionButton)
        {
            currentlyActiveButton = ButtonID.Q;
            return;
        }
        else
        {
            currentlyActiveButton = (ButtonID)Array.IndexOf(answerButtons, button);
        }

        SetColorsAndDisable(currentlyActiveButton);

        bool wasCorrect = currentlyActiveButton == ButtonID.A;
        if (wasCorrect)
        {
            question.correctAnsweredCount++;

            PlayerLevel.GainXp(PlayerLevel.correctAnswerXp);

            // to do: if GameMode == DailyTask
            //dailyTaskHistoryTable.IncrementCorrectAnsweredCount();
        }
        else
        {
            if (!isBookmarkSet)
            {
                SetBookmarkIcon();
            }

            PlayerLevel.GainXp(PlayerLevel.falseAnswerXp);
        }

        question.totalAnsweredCount++;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Equals("LinearQuiz"))
        {
            CatalogueSessionHistory currentSession = Global.GetCatalogue().sessionHistories.Last();

            // to do:
            int newAnswerId = question.answerHistory.Count;

            question.answerHistory.Add(new AnswerHistory(newAnswerId, question.id, DateTime.Now, wasCorrect, currentSession.id));
        }

        Global.UpdateQuestion(question);

        parentScreenManager.BroadcastMessage("EventButtonPressedCallback", currentlyActiveButton);
    }
    
    
    public void SetInitialBookmarkIcon()
    {
        Image img = bookmarkIcon.GetComponent<Image>();

        if (isBookmarkSet)
            img.color = bookmarkActiveColor;
        else
            img.color = Color.white;
    }


    public void SetBookmarkIcon()
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
        Global.UpdateQuestion(question);
    }


    public void SetContents(DataManager.QuestionResult result)
    {
    /* to do: evaluation
        question = catalogueTable.FindQuestionById(result.questionId);
        if (question != null)
            isBookmarkSet = question.enabledForPractice;
        else
            isBookmarkSet = false;

        SetInitialBookmarkIcon();
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
    */
    }
}

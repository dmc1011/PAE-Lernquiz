using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizAreaManager : MonoBehaviour
{

    [SerializeField] private Button questionButton;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private MonoBehaviour parentScreenManager;

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();
    private List<RectTransform> answerButtonTransforms = new List<RectTransform>();
    private ColorBlock defaultColorBlock;

    // The enum is used so "button objects" can solely stay in the QuizAreaManager.
    // while notifying the outside world about which buttons were pressed is still possible.
    public enum ButtonID { A /* = 0 -> correct answer */, B, C, D, Q, NONE }; 
    private ButtonID currentlyActiveButton = ButtonID.NONE;

    // Start is called before the first frame update
    void Start()
    {
        // Get components for questionButton
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();

        // Get components for answer buttons and add them to the lists
        foreach (Button button in answerButtons)
        {
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());
            answerButtonTransforms.Add(button.transform.GetComponent<RectTransform>());
        }

        // Get the default color of a question button
        defaultColorBlock = answerButtons[0].colors;
    }

    // Set the contents of all buttons
    public void SetContents(Question q)
    {
        questionButtonLabel.text = q.text;

        for (int i = 0; i < answerButtons.Length; i++)
            answerButtonLabels[i].text = q.answers[i].text;

    }

    // Reset the contents of all buttons
    public void ResetContents()
    {
        for(int i = 0; i < answerButtons.Length; i++)
        {
            answerButtonLabels[i].text = "";
            answerButtons[i].colors = defaultColorBlock;
            answerButtons[i].interactable = true;
        }
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
        if(button == questionButton)
            currentlyActiveButton = ButtonID.Q;
        else
            currentlyActiveButton = (ButtonID)Array.IndexOf(answerButtons, button);

        // There is currently no logic involved if someone uses the questionbutton.
        // this might be useful in the future (e.g. for giving hints? or something?) though.
        if (currentlyActiveButton == ButtonID.Q)
            return;

        // Always color button A in green.
        ColorBlock cb = button.colors;
        cb.disabledColor = Color.green;
        answerButtons[0].colors = cb;

        if (currentlyActiveButton != ButtonID.A) // right answer
        {
            cb.disabledColor = Color.red;
            button.colors = cb;
        }

        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].interactable = false;

        // MS: Ich weiß wirklich nicht ob das "der richtige" weg ist wie man in Unity Callbacks
        // veranstaltet... Wenn sich hier jemand auskennt kann er/sie diesen Kommentar entfernen und das ändern.
        parentScreenManager.BroadcastMessage("EventButtonPressedCallback", currentlyActiveButton);
        
    }

}

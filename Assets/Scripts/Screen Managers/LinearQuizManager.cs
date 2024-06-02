using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LinearQuizManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private Button QButton;
    [SerializeField] private Button AButton1;
    [SerializeField] private Button AButton2;
    [SerializeField] private Button AButton3;
    [SerializeField] private Button AButton4;
    [SerializeField] private Button NextButton;
    private TextMeshProUGUI QButton_Label;
    private TextMeshProUGUI AButton1_Label;
    private TextMeshProUGUI AButton2_Label;
    private TextMeshProUGUI AButton3_Label;
    private TextMeshProUGUI AButton4_Label;
    //private RectTransform QButton_Transform;
    private RectTransform AButton1_Transform;
    private RectTransform AButton2_Transform;
    private RectTransform AButton3_Transform;
    private RectTransform AButton4_Transform;

    private JsonDataService DataService = new JsonDataService();
    private Catalogue currentCatalogue;
    private int nextQuestionIndex = 0;
    ColorBlock defaultColorBlock;


    // Start is called before the first frame update
    void Start()
    {
        QButton_Label = QButton.GetComponentInChildren<TextMeshProUGUI>();
        AButton1_Label = AButton1.GetComponentInChildren<TextMeshProUGUI>();
        AButton2_Label = AButton2.GetComponentInChildren<TextMeshProUGUI>();
        AButton3_Label = AButton3.GetComponentInChildren<TextMeshProUGUI>();
        AButton4_Label = AButton4.GetComponentInChildren<TextMeshProUGUI>();
        //QButton_Transform = QButton.transform.GetComponent<RectTransform>();
        AButton1_Transform = AButton1.transform.GetComponent<RectTransform>();
        AButton2_Transform = AButton2.transform.GetComponent<RectTransform>();
        AButton3_Transform = AButton3.transform.GetComponent<RectTransform>();
        AButton4_Transform = AButton4.transform.GetComponent<RectTransform>();
        currentCatalogue = DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json");
        defaultColorBlock = AButton1.colors;
        NextButton.interactable = false;

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
        Vector3[] positions = {
            AButton1_Transform.position,
            AButton2_Transform.position,
            AButton3_Transform.position,
            AButton4_Transform.position
        };
        Functions.Shuffle(positions);
        AButton1_Transform.Translate(positions[0] - AButton1_Transform.position);
        AButton2_Transform.Translate(positions[1] - AButton2_Transform.position);
        AButton3_Transform.Translate(positions[2] - AButton3_Transform.position);
        AButton4_Transform.Translate(positions[3] - AButton4_Transform.position);
    }

    private void SetContents(Question q)
    {
        QButton_Label.text = q.text;
        AButton1_Label.text = q.answers[0].text;
        AButton2_Label.text = q.answers[1].text;
        AButton3_Label.text = q.answers[2].text;
        AButton4_Label.text = q.answers[3].text;
        
        Fragenummer.text =
            currentCatalogue.name + "\n" +
            "Frage " + q.id;
       
    }

    public void HighlightAnswer(Button button)
    {
        ColorBlock cb = button.colors;
        //cb.normalColor = Color.green;
        //cb.highlightedColor = Color.green;
        //cb.pressedColor = Color.green;
        //cb.selectedColor = Color.green;
        cb.disabledColor = Color.green;
        AButton1.colors = cb;

        if (button != AButton1)
        {

            //cb.normalColor = Color.red;
            //cb.highlightedColor = Color.red; 
            //cb.pressedColor = Color.red;
            //cb.selectedColor = Color.red;
            cb.disabledColor = Color.red;

            button.colors = cb;
        }

        AButton1.interactable = false;
        AButton2.interactable = false;
        AButton3.interactable = false;
        AButton4.interactable = false;
        NextButton.interactable = true;
    }

    private void ResetButtons()
    {
        AButton1.colors = defaultColorBlock;
        AButton2.colors = defaultColorBlock;
        AButton3.colors = defaultColorBlock;
        AButton4.colors = defaultColorBlock;
        AButton1.interactable = true;
        AButton2.interactable = true;
        AButton3.interactable = true;
        AButton4.interactable = true;
        NextButton.interactable = false;
    }

    public void QuitQuizRound()
    {
        SceneManager.LoadScene("Evaluation");
    }


}

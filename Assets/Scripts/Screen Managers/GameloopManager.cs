using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameloopManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private UnityEngine.UI.Button QButton;
    [SerializeField] private UnityEngine.UI.Button AButton1;
    [SerializeField] private UnityEngine.UI.Button AButton2;
    [SerializeField] private UnityEngine.UI.Button AButton3;
    [SerializeField] private UnityEngine.UI.Button AButton4;
    private TextMeshProUGUI QButton_Label;
    private TextMeshProUGUI AButton1_Label;
    private TextMeshProUGUI AButton2_Label;
    private TextMeshProUGUI AButton3_Label;
    private TextMeshProUGUI AButton4_Label;
    private RectTransform QButton_Transform;
    private RectTransform AButton1_Transform;
    private RectTransform AButton2_Transform;
    private RectTransform AButton3_Transform;
    private RectTransform AButton4_Transform;

    private JsonDataService DataService = new JsonDataService();

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Gameloop")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        QButton_Label = QButton.GetComponentInChildren<TextMeshProUGUI>();
        AButton1_Label = AButton1.GetComponentInChildren<TextMeshProUGUI>();
        AButton2_Label = AButton2.GetComponentInChildren<TextMeshProUGUI>();
        AButton3_Label = AButton3.GetComponentInChildren<TextMeshProUGUI>();
        AButton4_Label = AButton4.GetComponentInChildren<TextMeshProUGUI>();
        QButton_Transform = QButton.transform.GetComponent<RectTransform>();
        AButton1_Transform = AButton1.transform.GetComponent<RectTransform>();
        AButton2_Transform = AButton2.transform.GetComponent<RectTransform>();
        AButton3_Transform = AButton3.transform.GetComponent<RectTransform>();
        AButton4_Transform = AButton4.transform.GetComponent<RectTransform>();
        SetRandomizedPositions();
        SetContents();
    }

    public void Advance()
    {
        if (!Global.InsideQuestionRound) {
            print("This will not work, you are not inside a question round.");
            return;
        }
        if (Global.CurrentQuestionRound.QuestionCounter == Global.NumQuestionsPerRound - 1)
        {
            // Alle Fragen beantwortet
            SceneManager.LoadScene("Evaluation");
        } else
        {
            Global.CurrentQuestionRound.QuestionCounter += 1;
            SetContents();
            SetRandomizedPositions();
        }
    }

    public void AButton1ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
    }

    public void AButton2ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
    }

    public void AButton3ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
    }

    public void AButton4ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
    }

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

    private void SetContents()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        Catalogue currentCatalogue = DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json");
        Question currentQuestion = currentCatalogue.questions[Global.CurrentQuestionRound.Questions[Global.CurrentQuestionRound.QuestionCounter]];
        Debug.Log("The current Catalogue: " + currentCatalogue.name);
        Debug.Log("The current Question: " + currentQuestion.id);
        QButton_Label.text = currentQuestion.text;
        AButton1_Label.text = currentQuestion.answers[0].text;
        AButton2_Label.text = currentQuestion.answers[1].text;
        AButton3_Label.text = currentQuestion.answers[2].text;
        AButton4_Label.text = currentQuestion.answers[3].text;
        Fragenummer.text = 
            "Frage " + Global.CurrentQuestionRound.QuestionCounter + "\n" + 
            "(Katalog: " + Global.CurrentQuestionRound.CatalogueIndex + 
            ", Frage: " + Global.CurrentQuestionRound.Questions[Global.CurrentQuestionRound.QuestionCounter] + 
            ")";
    }

    private bool CurrentQuestionIsOutOfBounds()
    {
        if(!Global.InsideQuestionRound)
        {
            print("ERROR [NewGameManager.cs:Start()]: Global.InsideFragerunde == false, wie bist du überhaupt hier gelandet?!");
            return true;
        }
        if (Global.CurrentQuestionRound.CatalogueIndex >= DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory))
        {
            print("ERROR [ButtonManager.cs.SetContents()]: Global.AktuelleFragerunde.CatalogueIndex >= DataManager.Storage.Catalogues.Count");
            return true;
        }
        if (Global.CurrentQuestionRound.QuestionCounter >= Global.CurrentQuestionRound.Questions.Count)
        {
            print("ERROR [ButtonManager.cs.SetContents()]: Global.AktuelleFragerunde.QuestionCounter >= Global.AktuelleFragerunde.Questions.Count");
            return true;
        }
        if (Global.CurrentQuestionRound.Questions[Global.CurrentQuestionRound.QuestionCounter] >= DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json").questions.Count)
        {
            print("ERROR [ButtonManager.cs.SetContents()]: Global.AktuelleFragerunde.Questions[Global.AktuelleFragerunde.QuestionCounter] >= DataManager.Storage.Catalogues[Global.AktuelleFragerunde.CatalogueIndex].questions.Count");
            return true;
        }
        return false;
    }

}

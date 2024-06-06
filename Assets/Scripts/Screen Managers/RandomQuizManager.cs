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
    [SerializeField] private TextMeshProUGUI nextButtonLabel;

    private TextMeshProUGUI questionButtonLabel;
    private List<TextMeshProUGUI> answerButtonLabels = new List<TextMeshProUGUI>();
    private List<RectTransform> answerButtonTransforms = new List<RectTransform>();

    private JsonDataService DataService = new JsonDataService();
    private int selected_answer = 0;
    private bool isQuizOver = false;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "RandomQuiz") // TO DO: Gameloop-Szene umbenennen in RandomQuiz
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than RandomQUiz");
        }
        questionButtonLabel = questionButton.GetComponentInChildren<TextMeshProUGUI>();
        
        foreach (Button button in answerButtons)
        {
            answerButtonLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());
            answerButtonTransforms.Add(button.transform.GetComponent<RectTransform>());
        }


        DisplayNextQuestion();
        SetContents();
    }


    public void DisplayNextQuestion()
    {
        if (!Global.InsideQuestionRound)
        {
            print("This will not work, you are not inside a question round.");
            return;
        }

        SetRandomizedPositions();
        //Global.CurrentQuestionRound.ChosenAnswers[Global.CurrentQuestionRound.QuestionCounter] = selected_answer;     // erst für Ergebnisse relevant
        if (Global.CurrentQuestionRound.QuestionCounter == Global.NumQuestionsPerRound - 1)
        {
            LoadNextScene();
        }
        else
        {
            if (Global.CurrentQuestionRound.QuestionCounter == Global.NumQuestionsPerRound - 2)
            {
                nextButtonLabel.text = "Beenden";
            }
            Global.CurrentQuestionRound.QuestionCounter += 1;
            SetContents();
            SetRandomizedPositions();
        }
    }


    public void LoadNextScene()
    {
        if (isQuizOver)
        {
            SceneManager.LoadScene("Evaluation");
        }
    }


    public void AButton1ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        selected_answer = 0;
    }

    public void AButton2ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        selected_answer = 1;
    }


    public void AButton3ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        selected_answer = 2;
    }

    public void AButton4ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        selected_answer = 3;
    }


    // Jinsi: Exactly same method as in LinearQuizManager.
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
        questionButtonLabel.text = currentQuestion.text;

        for (int i = 0; i < 4; i++)
        {
            answerButtonLabels[i].text = currentQuestion.answers[i].text;
        }

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
            print("ERROR [NewGameManager.cs:Start()]: Global.InsideFragerunde == false, wie bist du �berhaupt hier gelandet?!");
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

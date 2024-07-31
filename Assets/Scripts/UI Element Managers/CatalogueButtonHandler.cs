using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Global;
using UnityEngine.UI;

public class CatalogueButtonHandler : MonoBehaviour
{
    [SerializeField] private Button catalogueButton;
    [SerializeField] private Background bg = null;
    private string targetScene = "";


    public void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        if (bg != null)
        {
            float timeNeeded = bg.TriggerEndSequence();
            print(bg);
            Invoke(nameof(LoadSceneInternal), timeNeeded);
        }
        else
        {
            LoadSceneInternal();
        }
    }


    private void LoadSceneInternal()
    {
        SceneManager.LoadScene(targetScene);
    }


    public void SetBackground(Background bg)
    {
        this.bg = bg;
    }


    // perform actions after selecting a catalogue, depending on chosen game mode
    public void OnCatalogueSelected()
    {
        switch (Global.CurrentQuestionRound.gameMode)
        {
            case Global.GameMode.LinearQuiz:
                StartLinearRoundClickedEvent();
                break;
            case Global.GameMode.RandomQuiz:
                StartRandomRoundClickedEvent();
                break;
            case Global.GameMode.Statistics:
                ShowStatistics();
                break;
            default:
                break;
        }
    }


    // start linear quiz
    private void StartLinearRoundClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        // invalid catalogue index
        if (!NewGameManager.catalogues.Any(catalogue => catalogue.name == catalogueName))
        {
            print($"ERROR [NewGameManager.cs.StartLinearRoundClickedEvent()]: There is no catalogue called '{catalogueName}'");
            return;
        }

        // start quiz round
        Global.CurrentQuestionRound.catalogue = NewGameManager.catalogueTable.FindCatalogueByName(catalogueName);
        Global.InsideQuestionRound = true;
        LoadScene("LinearQuiz");
    }


    // start random quiz
    private void StartRandomRoundClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        // check if chosen catalogue index is out of bounds
        if (!NewGameManager.catalogues.Any(catalogue => catalogue.name == catalogueName))
        {
            print($"ERROR [NewGameManager.cs.StartRandomRoundClickedEvent()]: There is no catalogue called '{catalogueName}'");
            return;
        }

        // load chosen catalogue into global data
        Global.CurrentQuestionRound.catalogue = NewGameManager.catalogueTable.FindCatalogueByName(catalogueName);
        int catalogueSize = Global.CurrentQuestionRound.catalogue.questions.Count;

        // initialize question round
        Global.CurrentQuestionRound.questions = new();
        int[] iota = Enumerable.Range(0, catalogueSize).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        Global.CurrentQuestionRound.questionLimit = Mathf.Min(Global.RandomQuizSize, catalogueSize);
        for (int i = 0; i < Global.CurrentQuestionRound.questionLimit; i++) // select first n questions of randomized questions
        {
            Global.CurrentQuestionRound.questions.Add(iota[i]);
        }
        Global.InsideQuestionRound = true;
        LoadScene("RandomQuiz");
    }


    // start statistics
    private void ShowStatistics()
    {
        LoadScene("Statistics");
    }
}

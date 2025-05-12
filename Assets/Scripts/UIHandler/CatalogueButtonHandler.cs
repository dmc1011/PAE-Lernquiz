using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Global;
using UnityEngine.UI;
using Controllers;
using Repositories;
using UseCases;
using Client = Supabase.Client;
using Services;

public class CatalogueButtonHandler : MonoBehaviour
{
    public enum EditorAction
    {
        Add,
        Edit
    }

    [SerializeField] private Button catalogueButton;
    [SerializeField] private HexagonBackground bg = null;
    private string targetScene = "";

    private static Client _supabase;

    private ICatalogueRepository _catalogueRepo;

    private FetchCataloguesUseCase _cataloguesUseCase;

    private CatalogueController _catalogueController;

    public void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();
        _catalogueRepo = new SupabaseCatalogueRepository(_supabase);
        _cataloguesUseCase = new FetchCataloguesUseCase(_catalogueRepo);
        _catalogueController = new CatalogueController(_cataloguesUseCase);
    }

    public void OnClick()
    {
        switch (Global.CurrentQuestionRound.gameMode)
        {
            case GameMode.LinearQuiz:
                StartLinearRoundClickedEvent();
                break;
            case GameMode.RandomQuiz:
                StartRandomRoundClickedEvent();
                break;
            case GameMode.Statistics:
                ShowStatistics();
                break;
            case GameMode.Editor:
                StartEditor();
                break;
            case GameMode.PracticeBook:
                StartPracticeBookClickedEvent();
                break;
            default:
                break;
        }
    }

    private void StartLinearRoundClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        /*
        // invalid catalogue name
        if (!NewGameManager.catalogues.Any(catalogue => catalogue.name == catalogueName))
        {
            print($"ERROR [NewGameManager.cs.StartLinearRoundClickedEvent()]: There is no catalogue called '{catalogueName}'");
            return;
        }
        */

        try
        {
            _catalogueController.GetCatalogue
        }
        Global.CurrentQuestionRound.catalogue = NewGameManager.catalogueTable.FindCatalogueByName(catalogueName);
        Global.InsideQuestionRound = true;
        LoadScene("LinearQuiz");
    }


    // start random quiz
    private void StartRandomRoundClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        // invalid catalogue name
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
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        if (catalogueName == "Daily Task")
        {
            StatisticManager.isDailyTaskStatistic = true;
            LoadScene("Statistics");
            return;
        }

        // invalid catalogue name
        if (!NewGameManager.catalogues.Any(catalogue => catalogue.name == catalogueName))
        {
            print($"ERROR [NewGameManager.cs.StartLinearRoundClickedEvent()]: There is no catalogue called '{catalogueName}'");
            return;
        }

        // start statistics
        StatisticManager.isDailyTaskStatistic = false;
        CurrentQuestionRound.catalogue = NewGameManager.catalogueTable.FindCatalogueByName(catalogueName);

        LoadScene("Statistics");
    }
    

    // start editor
    private void StartEditor()
    {
        string catalogueName = this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        if (catalogueName == "Katalog hinzufügen" && Global.SetTmpCatalogue(null))
        {
            EditorManager.isNewCatalogue = true;
            LoadScene("Editor");
            return;
        }

        if (Global.SetTmpCatalogue(catalogueName))
        {
            EditorManager.isNewCatalogue = false;
            LoadScene("Editor");
        }
    }

    // start practice book
    private void StartPracticeBookClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        // invalid catalogue name
        if (!NewGameManager.catalogues.Any(catalogue => catalogue.name == catalogueName))
        {
            print($"ERROR [NewGameManager.cs.StartPracticeBookClickedEvent()]: There is no catalogue called '{catalogueName}'");
            return;
        }

        // start quiz round
        Global.CurrentQuestionRound.catalogue = NewGameManager.catalogueTable.FindCatalogueByName(catalogueName);
        Global.InsideQuestionRound = true;
        LoadScene("PractiseBook");
    }

    public void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        if (bg != null)
        {
            float timeNeeded = bg.TriggerEndSequence();
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


    public void SetBackground(HexagonBackground bg)
    {
        this.bg = bg;
    }
}

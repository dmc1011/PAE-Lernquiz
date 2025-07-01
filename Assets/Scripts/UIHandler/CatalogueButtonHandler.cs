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
using System;
using Entities;

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
    private SupabaseRequestUseCase _supabaseRequestUseCase;

    private CatalogueController _catalogueController;

    public void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();
        _catalogueRepo = new SupabaseCatalogueRepository(_supabase);
        _cataloguesUseCase = new FetchCataloguesUseCase(_catalogueRepo);
        _supabaseRequestUseCase = new SupabaseRequestUseCase(_catalogueRepo);
        _catalogueController = new CatalogueController(_cataloguesUseCase, _supabaseRequestUseCase);
    }

    public void OnClick()
    {
        Debug.Log(Global.CurrentQuestionRound.gameMode);

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
                if (Global.EditorType == SceneLoader.EditorType.Catalogue)
                {
                    StartCatalogueEditor();
                }
                else
                {
                    Debug.Log("Unsupported editor mode on CatalogueButtonHandler click.");
                }
                break;
            case GameMode.PracticeBook:
                StartPracticeBookClickedEvent();
                break;
            default:
                break;
        }
    }

    private async void StartLinearRoundClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        CatalogueDTO catalogue = ContentSelectionHandler.catalogues.Find(c => c.name == catalogueName);
        Debug.Log("Cat ID: " + catalogue?.id);
        Debug.Log("Catalogues count: " + ContentSelectionHandler.catalogues.Count);

        try
        {
            if (catalogue == null)
            {
                throw new FetchDataException("Fehler beim Starten des Quiz: Der ausgewählte Katalog existiert nicht im ContentHandler");
            }

            var catalogueResult = await _catalogueController.GetCatalogueById(catalogue.id);

            Debug.Log("Catalogue Details:");
            Debug.Log(catalogueResult.name);
            Debug.Log("Question count: " + catalogueResult.questions?.Count);

            if (catalogueResult == null)
            {
                throw new FetchDataException("Der ausgewählte Katalog wurde nicht gefunden");
            }

            // to do: replace with single method "InitializeQuestionRound"
            SetCatalogue(catalogueResult);
            SetInsideQuestionRound(true);
            ClearAnswerHistories();

            LoadScene("LinearQuiz");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    // start practice book
    private async void StartPracticeBookClickedEvent()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        CatalogueDTO catalogue = ContentSelectionHandler.catalogues.Find(c => c.name == catalogueName);
        Debug.Log("Cat ID: " + catalogue?.id);
        Debug.Log("Catalogues count: " + ContentSelectionHandler.catalogues.Count);

        try
        {
            if (catalogue == null)
            {
                throw new FetchDataException("Fehler beim Starten des Quiz: Der ausgewählte Katalog existiert nicht im ContentHandler");
            }

            var catalogueResult = await _catalogueController.GetCatalogueById(catalogue.id);

            if (catalogueResult == null)
            {
                throw new FetchDataException("Der ausgewählte Katalog wurde nicht gefunden");
            }

            SetCatalogue(catalogueResult);
            SetInsideQuestionRound(true);
            ClearAnswerHistories();

            LoadScene("PractiseBook");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    // start editor
    private async void StartCatalogueEditor()
    {
        string catalogueName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;
        //string catalogueName = this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        if (catalogueName == "Neuen Katalog hinzufügen" && Global.SetTmpCatalogue(null))
        {
            EditorManager.isNewCatalogue = true;
            LoadScene("Editor");
            return;
        }

        CatalogueDTO catalogue = ContentSelectionHandler.catalogues.Find(c => c.name == catalogueName);
        Debug.Log(catalogue?.id);

        try
        {
            if (catalogue == null)
            {
                throw new FetchDataException("Fehler beim Starten des Editors: Der ausgewählte Katalog existiert nicht im ContentHandler");
            }

            var catalogueResult = await _catalogueController.GetCatalogueById(catalogue.id);

            if (catalogueResult == null)
            {
                throw new FetchDataException("Der ausgewählte Katalog wurde nicht gefunden");
            }

            // to do: replace with single method "InitializeQuestionRound"
            SetCatalogue(catalogueResult);
            SetInsideQuestionRound(false);
            ClearAnswerHistories();

            if (Global.SetTmpCatalogue(catalogueResult))
            {
                EditorManager.isNewCatalogue = false;
                LoadScene("Editor");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /*
    private async void StartTopicEditor()
    {
        string topicName = catalogueButton.GetComponentInChildren<TextMeshProUGUI>().text;

        if (topicName == "Neues Thema hinzufügen")
        {
            EditorManager.isNewTopic = true;
            SetTmpTopic(null);

            LoadScene("Editor");
            return;
        }

        Topic topic = ContentSelectionHandler.topics.Find(t => t.name == topicName);

        EditorManager.isNewTopic = false;
        SetTmpTopic(topic);
        Debug.Log(topic?.name);
        LoadScene("Editor");
    }
    */





    // to do: rework from here


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

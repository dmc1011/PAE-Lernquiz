using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button startDailyTask;
    [SerializeField] private HexagonBackground background;

    private string targetScene;
    private int catalogueCount;
    CatalogueTable catalogueTable;
    List<Catalogue> catalogues;
    private string currentDate;


    void Start()
    {
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;

        PlayerLevel.FetchData();

        currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // reset daily task
        if (IsNewDay())
        {
            ResetDailyTask();
            Debug.Log("Daily Task reset");
        }
    }


    public void StartDailyTaskClickedEvent()
    {
        // show evaluation if daily task has already been completed
        if (IsDailyTaskCompleted())
        {
            Global.SetGameMode(Global.GameMode.DailyTask);
            LoadDailyTaskScene(Strings.Evaluation);
            return;
        }

        // load chosen catalogue into global data
        Catalogue randomCatalogue = catalogueTable.FindRandomCatalogue();
        LoadDailyTaskGlobally(randomCatalogue);

        // initialize daily task
        Global.CurrentDailyTask.questions = new();
        int[] iota = Enumerable.Range(0, Global.CurrentDailyTask.catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        Global.CurrentDailyTask.questionLimit = Mathf.Min(Global.DailyTaskSize, Global.CurrentDailyTask.catalogue.questions.Count);
        for (int i = 0; i < Global.CurrentDailyTask.questionLimit; i++) // select first n questions of randomized questions
        {
            Global.CurrentDailyTask.questions.Add(iota[i]);
        }

        // start daily task
        Global.InsideQuestionRound = true;
        LoadDailyTaskScene(Strings.DailyTask);
    }

    public void LoadDailyTaskGlobally(Catalogue catalogue) {
        Global.CurrentDailyTask.catalogueIndex = catalogue.id;
        Global.CurrentDailyTask.catalogue = catalogue;
        PlayerPrefsManager.SetDailyTaskCatalogueId();
    }


    private bool IsNewDay()
    {
        string lastResetDate = PlayerPrefsManager.GetLastResetDate();
        return currentDate != lastResetDate;
    }


    private void ResetDailyTask()
    {
        Global.CurrentDailyTask = new DataManager.DailyTask();
        PlayerPrefsManager.SetLastResetDate(currentDate);
        PlayerPrefsManager.SetIsDailyTaskCompleted(false);
        PlayerPrefs.Save();
    }


    private bool IsDailyTaskCompleted()
    {
        return PlayerPrefsManager.GetIsDailyTaskCompleted();
    }


// to do: auslagern in Scene Loader;
    public void LoadDailyTaskScene(string sceneName)
    {
        targetScene = sceneName;
        Global.CurrentQuestionRound.gameMode = Global.GameMode.DailyTask;
        StartSceneTransition();
    }

    public void LoadProfileScene()
    {
        targetScene = Strings.Profile;
        StartSceneTransition(false);
    }


    public void LoadHelpScene()
    {
        targetScene = Strings.Help;
        StartSceneTransition(false);
    }

    public void LoadLinearGameSelection()
    {
        targetScene = Strings.NewGame;
        Global.CurrentQuestionRound.gameMode = Global.GameMode.LinearQuiz;
        StartSceneTransition();
    }


    public void LoadRandomGameSelection()
    {
        targetScene = Strings.NewGame;
        Global.CurrentQuestionRound.gameMode = Global.GameMode.RandomQuiz;
        StartSceneTransition();
    }


    public void LoadStatisticsSelection()
    {
        targetScene = Strings.NewGame;
        Global.CurrentQuestionRound.gameMode = Global.GameMode.Statistics;
        StartSceneTransition();
    }


    public void LoadEditorSelection()
    {
        targetScene = Strings.NewGame;
        Global.CurrentQuestionRound.gameMode = Global.GameMode.Editor;
        StartSceneTransition(false);
    }
    
    
    public void LoadPracticeBookSelection()
    {
        targetScene = Strings.NewGame;
        Global.CurrentQuestionRound.gameMode = Global.GameMode.PracticeBook;
        StartSceneTransition();
    }


    private void StartSceneTransition(bool useAnimation = true)
    {
        if (background != null && useAnimation)
        {
            float timeNeeded = background.TriggerEndSequence();
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
}

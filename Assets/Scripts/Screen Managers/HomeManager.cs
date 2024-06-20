using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{

    [SerializeField] private Button startDailyTask;
    JsonDataService dataService = new JsonDataService();
    private int catalogueCount;

    // key for last reset date in player prefs
    string currentDate;

    void Start()
    {
        catalogueCount = dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory);
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        if (IsNewDay())
        {
            ResetDailyTask();
            Debug.Log("Daily Task reset successfully");
        }
    }

  
    public void StartDailyTaskClickedEvent()
    {
        if (Global.isDailyTaskCompleted)
        {
            SceneManager.LoadScene("Evaluation");
            return;
        }
        Global.CurrentDailyTask.catalogueIndex = UnityEngine.Random.Range(0, catalogueCount);

        // load chosen catalogue into global data
        Global.CurrentDailyTask.catalogue = dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentDailyTask.catalogueIndex}.json");

        // initialize question round
        Global.CurrentDailyTask.questions = new();
        int[] iota = Enumerable.Range(0, Global.CurrentDailyTask.catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        Global.CurrentDailyTask.questionLimit = 10;
        for (int i = 0; i <  Global.CurrentDailyTask.questionLimit; i++) // select first n questions of randomized questions
        {
            Global.CurrentDailyTask.questions.Add(iota[i]);
            Debug.Log($"{i}");
        }
        Global.InsideQuestionRound = true;
        PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "true");
        SceneManager.LoadScene("DailyTask");
    }

    public bool IsNewDay()
    {
        string currentDate = DateTime.Now.ToString("yyy-MM-dd");
        string lastResetDate = PlayerPrefs.GetString(Global.LastResetDateKey, "");
        if (currentDate != lastResetDate)
        {
            return true;
        }
        return false;
    }


    public void ResetDailyTask()
    {
        Global.CurrentDailyTask = new DataManager.DailyTask();
        PlayerPrefs.SetString(Global.LastResetDateKey, currentDate);
        PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "false");
        PlayerPrefs.Save();
    }
}

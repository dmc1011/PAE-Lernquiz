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
    string currentDate;


    void Start()
    {
        catalogueCount = dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory);
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        if (IsNewDay())
        {
            ResetDailyTask();
        }
    }

  
    public void StartDailyTaskClickedEvent()
    {
        if (IsDailyTaskCompleted())
        {
            SceneManager.LoadScene("Evaluation");
            return;
        }

        // load chosen catalogue into global data
        Global.CurrentDailyTask.catalogueIndex = UnityEngine.Random.Range(0, catalogueCount);
        Global.CurrentDailyTask.catalogue = dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentDailyTask.catalogueIndex}.json");

        // initialize daily task
        Global.CurrentDailyTask.questions = new();
        int[] iota = Enumerable.Range(0, Global.CurrentDailyTask.catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        Global.CurrentDailyTask.questionLimit = Mathf.Min(10, Global.CurrentDailyTask.catalogue.questions.Count);
        for (int i = 0; i < Global.CurrentDailyTask.questionLimit; i++) // select first n questions of randomized questions
        {
            Global.CurrentDailyTask.questions.Add(iota[i]);
            Debug.Log($"{i}");
        }

        // start daily task
        Global.InsideQuestionRound = true;
        PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "true");
        PlayerPrefs.Save();
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


    public bool IsDailyTaskCompleted()
    {
        return PlayerPrefs.GetString(Global.IsDailyTaskCompletedKey) == "true"; ;
    }
}

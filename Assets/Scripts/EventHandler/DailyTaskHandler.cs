using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DailyTaskHandler : MonoBehaviour
{
    [SerializeField] private HexagonBackground background;
    [SerializeField] private SceneLoader sceneLoader;

    private string currentDate;

    // Start is called before the first frame update
    void Start()
    {
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // reset daily task
        if (IsNewDay())
        {
            ResetDailyTask();
            Debug.Log("Daily Task reset");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void StartDailyTaskClickedEvent()
    {
        // show evaluation if daily task has already been completed
        if (IsDailyTaskCompleted())
        {
            Global.SetGameMode(GameMode.DailyTask);
            sceneLoader.LoadScene(Scene.Evaluation);
            return;
        }

        // to do: load random catalogue into global data
        //Catalogue randomCatalogue = catalogueTable.FindRandomCatalogue();
        //LoadDailyTaskGlobally(randomCatalogue);

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
        sceneLoader.LoadSceneWithGameMode(Scene.DailyTask, GameMode.DailyTask, true);
    }
}

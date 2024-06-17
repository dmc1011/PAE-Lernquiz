using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{


    [SerializeField] private Button startDailyTask;

    JsonDataService dataService = new JsonDataService();
    private int catalogueCount;
    void Start()
        {
            catalogueCount = dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory);
        }

  
    public void StartDailyTaskClickedEvent()
    {
        // check if chosen catalogue index is out of bounds
        if (Global.CurrentDailyTask.catalogueIndex >= catalogueCount)
        {
            print("ERROR [HomeManager.cs.StartDailyTaskClickedEvent()]: Fragerunde mit Katalognummer " 
                + Global.CurrentDailyTask.catalogueIndex + " ist OutOfBounds. Es gibt " 
                + catalogueCount + " Fragenkataloge.");
            return;
        }

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
        }
         // number of questions that have to be answered in a random quiz
        Global.InsideQuestionRound = true;
        SceneManager.LoadScene("DailyTask");
    }
}

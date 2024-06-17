using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown catalogueSelection;
    [SerializeField] private Button startLinearRound;
    [SerializeField] private Button startRandomRound;

    JsonDataService dataService = new JsonDataService();
    private int catalogueCount;

    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        catalogueCount = dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory);
        SetContents();
    }

    private void SetContents()
    {
        catalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < catalogueCount; i++)
        {
            options.Add(new(dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{i}.json").name));
        }
        if (options.Count == 0)
        {
            options.Add(new("Nicht verfügbar"));
        }
        catalogueSelection.AddOptions(options);
        CatalogueSelectionChangedEvent();
    }

    public void CatalogueSelectionChangedEvent()
    {
        if (catalogueCount == 0)
        {
            Global.CurrentQuestionRound.catalogueIndex = 0;
            startLinearRound.interactable = false;
            startRandomRound.interactable = false;
        }
        else
        {
            Global.CurrentQuestionRound.catalogueIndex = catalogueSelection.value;
            startLinearRound.interactable = true;
            startRandomRound.interactable = true;
        }
    }

    public void StartLinearRoundClickedEvent()
    {
        // invalid catalogue index
        if (Global.CurrentQuestionRound.catalogueIndex >= catalogueCount)
        {
            print("ERROR: Fragerunde mit Katalognummer " + Global.CurrentQuestionRound.catalogueIndex + " ist OutOfBounds. Es gibt " + catalogueCount + " Fragenkataloge.");
            return;
        }

        // start quiz round

        CatalogueTable catalogueTable = new CatalogueTable();
        string catalogueId = Global.CurrentQuestionRound.catalogueIndex.ToString();
        Debug.Log("Table: " + catalogueTable);
        Global.CurrentQuestionRound.catalogue = catalogueTable.GetCatalogueById(catalogueId);
        Global.InsideQuestionRound = true;
        SceneManager.LoadScene("LinearQuiz");
    }

    public void StartRandomRoundClickedEvent()
    {
        // check if chosen catalogue index is out of bounds
        if (Global.CurrentQuestionRound.catalogueIndex >= catalogueCount)
        {
            print("ERROR [NewGameManager.cs.StartZufallsRundeClickedEvent()]: Fragerunde mit Katalognummer " 
                + Global.CurrentQuestionRound.catalogueIndex + " ist OutOfBounds. Es gibt " 
                + catalogueCount + " Fragenkataloge.");
            return;
        }

        // load chosen catalogue into global data
        Global.CurrentQuestionRound.catalogue = dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.catalogueIndex}.json");

        // initialize question round
        Global.CurrentQuestionRound.questions = new();
        int[] iota = Enumerable.Range(0, Global.CurrentQuestionRound.catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        for (int i = 0; i < Global.NumQuestionsPerRound; i++) // select first n questions of randomized questions
        {
            Global.CurrentQuestionRound.questions.Add(iota[i]);
        }
        Global.CurrentQuestionRound.questionLimit = 5; // number of questions that have to be answered in a random quiz
        Global.InsideQuestionRound = true;
        SceneManager.LoadScene("RandomQuiz");
    }

}

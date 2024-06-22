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

    private int catalogueCount;
    CatalogueTable catalogueTable;
    List<Catalogue> catalogues;

    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;
        SetContents();
    }

    private void SetContents()
    {
        catalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < catalogues.Count; i++)
        {
            Catalogue catalogue = catalogues[i];
            options.Add(new(catalogue.name));
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
            Global.CurrentQuestionRound.catalogueIndex = catalogueTable.FindCatalogueByName(catalogueSelection.options[catalogueSelection.value].text).id;
            startLinearRound.interactable = true;
            startRandomRound.interactable = true;
        }
    }

    public void StartLinearRoundClickedEvent()
    {
        // invalid catalogue index
        if (!catalogues.Any(catalogue => catalogue.id == Global.CurrentQuestionRound.catalogueIndex))
        {
            print("ERROR: Fragerunde mit Katalognummer " + Global.CurrentQuestionRound.catalogueIndex + " ist OutOfBounds. Es gibt " + catalogueCount + " Fragenkataloge.");
            return;
        }

        // start quiz round
        Global.CurrentQuestionRound.catalogue = catalogueTable.FindCatalogueById(Global.CurrentQuestionRound.catalogueIndex);
        Global.InsideQuestionRound = true;
        SceneManager.LoadScene("LinearQuiz");
    }

    public void StartRandomRoundClickedEvent()
    {
        // check if chosen catalogue index is out of bounds
        if (!catalogues.Any(catalogue => catalogue.id == Global.CurrentQuestionRound.catalogueIndex))
        {
            print("ERROR [NewGameManager.cs.StartZufallsRundeClickedEvent()]: Fragerunde mit Katalognummer " 
                + Global.CurrentQuestionRound.catalogueIndex + " ist OutOfBounds. Es gibt " 
                + catalogueCount + " Fragenkataloge.");
            return;
        }

        // load chosen catalogue into global data
        Global.CurrentQuestionRound.catalogue = catalogueTable.FindCatalogueById(Global.CurrentQuestionRound.catalogueIndex);

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

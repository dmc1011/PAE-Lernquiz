using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown CatalogueSelection;
    [SerializeField] private Button StartLinearRound;
    [SerializeField] private Button StartRandomRound;

    JsonDataService DataService = new JsonDataService();

    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        SetContents();
    }

    private void SetContents()
    {
        CatalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory); i++)
        {
            options.Add(new(DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{i}.json").name));
        }
        if (options.Count == 0)
        {
            options.Add(new("Nicht verfügbar"));
        }
        CatalogueSelection.AddOptions(options);
        CatalogueSelectionChangedEvent();
    }

    public void CatalogueSelectionChangedEvent()
    {
        if (DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory) == 0)
        {
            Global.CurrentQuestionRound.CatalogueIndex = 0;
            StartLinearRound.interactable = false;
            StartRandomRound.interactable = false;
        }
        else
        {
            Global.CurrentQuestionRound.CatalogueIndex = CatalogueSelection.value;
            StartLinearRound.interactable = true;
            StartRandomRound.interactable = true;
        }
    }

    public void StartLinearRoundClickedEvent()
    {
        // invalid catalogue index
        if (Global.CurrentQuestionRound.CatalogueIndex >= DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory))
        {
            print("ERROR: Fragerunde mit Katalognummer " + Global.CurrentQuestionRound.CatalogueIndex + " ist OutOfBounds. Es gibt " + DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory) + " Fragenkataloge.");
            return;
        }

        // chosen catalogue
        Catalogue catalogue = DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json");

        // start quiz round
        Global.InsideQuestionRound = true;
        SceneManager.LoadScene("LinearQuiz");
    }

    public void StartRandomRoundClickedEvent()
    {
        // Hier werden die Fragen aus dem Fragenkatalog ausgewählt und zusammengestellt.
        if (Global.CurrentQuestionRound.CatalogueIndex >= DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory))
        {
            print("ERROR [NewGameManager.cs.StartZufallsRundeClickedEvent()]: Fragerunde mit Katalognummer " + Global.CurrentQuestionRound.CatalogueIndex + " ist OutOfBounds. Es gibt " + DataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory) + " Fragenkataloge.");
            return;
        }

        // Das hier ist der ausgewählte Katalog
        Catalogue catalogue = DataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.CatalogueIndex}.json");

        // Die Parameter der Fragerunde festlegen -> Welche Fragen kommen und wie viele. Listen für Fragen & Antworten etc. initialisieren.
        Global.CurrentQuestionRound.Questions = new();
        int[] iota = Enumerable.Range(0, catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1]
        Functions.Shuffle(iota); // Zufallszahlen ohne Dopplungen. Easy.
        for (int i = 0; i < Global.NumQuestionsPerRound; i++) // Fügt die ersten N hinzu.
        {
            Global.CurrentQuestionRound.Questions.Add(iota[i]);
        }
        Global.CurrentQuestionRound.QuestionCounter = 0; // Wir starten von 0 und gehen bis Global.NumQuestionsPerRound - 1
        Global.CurrentQuestionRound.QuestionLimit = 5;
        Global.InsideQuestionRound = true; // Schalter umlegen
        SceneManager.LoadScene("Gameloop"); // Lets go.
    }

}

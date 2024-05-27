using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown Katalogauswahl;
    [SerializeField] private UnityEngine.UI.Button StartLineareRunde;
    [SerializeField] private UnityEngine.UI.Button StartZufallsRunde;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Screen_NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        SetContents();
    }

    private void SetContents()
    {
        Katalogauswahl.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < DataManager.Storage.Catalogues.Count; i++)
        {
            options.Add(new(DataManager.Storage.Catalogues[i].name));
        }
        if (options.Count == 0)
        {
            options.Add(new("Nicht verfügbar"));
        }
        Katalogauswahl.AddOptions(options);
        KatalogauswahlChangedEvent();
    }

    public void KatalogauswahlChangedEvent()
    {
        if (DataManager.Storage.Catalogues.Count == 0)
        {
            Global.AktuelleFragerunde.CatalogueIndex = 0;
            StartLineareRunde.interactable = false;
            StartZufallsRunde.interactable = false;
        }
        else
        {
            Global.AktuelleFragerunde.CatalogueIndex = Katalogauswahl.value;
            StartLineareRunde.interactable = true;
            StartZufallsRunde.interactable = true;
        }
    }

    public void StartLineareRundeClickedEvent()
    {
        print("TODO: Runde in der der Fragenkatalog linear durchlaufen wird");
    }

    public void StartZufallsRundeClickedEvent()
    {
        // Hier werden die Fragen aus dem Fragenkatalog ausgewählt und zusammengestellt.
        if (Global.AktuelleFragerunde.CatalogueIndex >= DataManager.Storage.Catalogues.Count)
        {
            print("ERROR [NewGameManager.cs.StartZufallsRundeClickedEvent()]: Fragerunde mit Katalognummer " + Global.AktuelleFragerunde.CatalogueIndex + " ist OutOfBounds. Es gibt " + DataManager.Storage.Catalogues.Count + " Fragenkataloge.");
            return;
        }

        // Das hier ist der ausgewählte Katalog
        DataManager.Catalogue catalogue = DataManager.Storage.Catalogues[Global.AktuelleFragerunde.CatalogueIndex];

        // Die Parameter der Fragerunde festlegen -> Welche Fragen kommen und wie viele. Listen für Fragen & Antworten etc. initialisieren.
        Global.AktuelleFragerunde.Questions = new();
        Global.AktuelleFragerunde.ChosenAnswers = new();
        int[] iota = Enumerable.Range(0, catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1]
        Functions.Shuffle(iota); // Zufallszahlen ohne Dopplungen. Easy.
        for (int i = 0; i < Global.NumQuestionsPerRound; i++) // Fügt die ersten N hinzu.
        {
            Global.AktuelleFragerunde.Questions.Add(iota[i]);
            Global.AktuelleFragerunde.ChosenAnswers.Add(-1); // keine Antwort ausgewählt
        }
        Global.AktuelleFragerunde.QuestionCounter = 0; // Wir starten von 0 und gehen bis Global.NumQuestionsPerRound - 1
        Global.InsideFragerunde = true; // Schalter umlegen
        SceneManager.LoadScene("Screen_SingleplayerGameloop_1"); // Lets go.
    }

}

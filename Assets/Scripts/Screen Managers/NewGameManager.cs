using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{
    [SerializeField] private GameObject catalogueButtonPrefab;      // used for dynamically rendering catalogue buttons
    [SerializeField] private Transform buttonContainer;             // 'content' element of scroll view
    [SerializeField] private Background bg = null;

    [HideInInspector] public static CatalogueTable catalogueTable;
    [HideInInspector] public static int catalogueCount;
    [HideInInspector] public static List<Catalogue> catalogues;
    [HideInInspector] public static Global.GameMode gameMode;

    void Start()
    {
        // Failsafe
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any other scene than 'NewGame'.");
            return;
        }

        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;
        gameMode = Global.CurrentQuestionRound.gameMode;       // get current GameMode: defines behavior of events triggered by selecting a catalogue

        SetCatalogueButtons();
    }

    // renders a catalogue button for each existing catalogue on the scroll view element
    private void SetCatalogueButtons()
    {
        for (int i = 0; i < catalogueCount; i++)
        {
            GameObject catalogueButton = Instantiate(catalogueButtonPrefab, buttonContainer);

            // set background on runtime
            var manager = catalogueButton.GetComponent<CatalogueButtonHandler>();
            manager.SetBackground(bg);

            // display catalogue name on button
            TextMeshProUGUI buttonLabel = catalogueButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = catalogues[i].name;
        }
    }
}

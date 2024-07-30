using System.Collections.Generic;
//using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{

    //[SerializeField] private TMP_Dropdown catalogueSelection;
    //[SerializeField] private Button startLinearRound;
    //[SerializeField] private ButtonNavigation startLinearRoundNavigation;
    //[SerializeField] private Button startRandomRound;
    //[SerializeField] private ButtonNavigation startRandomRoundNavigation;

    [SerializeField] private GameObject catalogueButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Background bg = null;

    [HideInInspector] public static CatalogueTable catalogueTable;
    [HideInInspector] public static int catalogueCount;
    [HideInInspector] public static List<Catalogue> catalogues;
    [HideInInspector] public static Global.GameMode gameMode;

    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than 'NewGame'.");
            return;
        }

        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;
        gameMode = Global.CurrentQuestionRound.gameMode;

        SetCatalogueButtons();
    }

    private void SetCatalogueButtons()
    {
        for (int i = 0; i < catalogueCount; i++)
        {
            GameObject catalogueButton = Instantiate(catalogueButtonPrefab, buttonContainer);
            var manager = catalogueButton.GetComponent<CatalogueButtonHandler>();
            manager.SetBackground(bg);
            TextMeshProUGUI buttonLabel = catalogueButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = catalogues[i].name;
        }
    }
}

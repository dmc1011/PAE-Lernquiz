using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CataloguesManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown catalogueSelection;
    [SerializeField] private TMP_Dropdown questionSelection;
    [SerializeField] private UnityEngine.UI.Button QButton;
    [SerializeField] private UnityEngine.UI.Button AButton1;
    [SerializeField] private UnityEngine.UI.Button AButton2;
    [SerializeField] private UnityEngine.UI.Button AButton3;
    [SerializeField] private UnityEngine.UI.Button AButton4;
    private TextMeshProUGUI QButton_Label;
    private TextMeshProUGUI AButton1_Label;
    private TextMeshProUGUI AButton2_Label;
    private TextMeshProUGUI AButton3_Label;
    private TextMeshProUGUI AButton4_Label;


    private JsonDataService dataService = new JsonDataService();

    void Start()
    {
        if(SceneManager.GetActiveScene().name != "Catalogues")
        {
            print("ERROR [CataloguesManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        QButton_Label = QButton.GetComponentInChildren<TextMeshProUGUI>();
        AButton1_Label = AButton1.GetComponentInChildren<TextMeshProUGUI>();
        AButton2_Label = AButton2.GetComponentInChildren<TextMeshProUGUI>();
        AButton3_Label = AButton3.GetComponentInChildren<TextMeshProUGUI>();
        AButton4_Label = AButton4.GetComponentInChildren<TextMeshProUGUI>();
        SetContents();
    }

    private void SetContents()
    {
        catalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory); i++)
        {
            options.Add(new(dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{i}.json").name));
        }
        if (options.Count == 0)
        {
            options.Add(new("Nicht verfügbar"));
        }
        catalogueSelection.AddOptions(options);
        CatalogueSelectionChangedEvent(); // This will update QuestionSelection
    }

    public void CatalogueSelectionChangedEvent()
    {
        questionSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        if (dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory) != 0)
        {
            for (int i = 0; i < dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{catalogueSelection.value}.json").questions.Count; i++)
            {
                options.Add(new(dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{catalogueSelection.value}.json").questions[i].text));
            }
        }
        if (options.Count == 0)
        {
            options.Add(new("-"));
        }
        questionSelection.AddOptions(options);
        QuestionSelectionChangedEvent();
    }

    public void QuestionSelectionChangedEvent()
    {
        if (dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory) == 0)
        {
            print("ERROR [CataloguesManager.cs:QuestionSelectionChangedEvent()]: Wir benötigen mehr Fragenkataloge, Milord.");
            return;
        }
        if (dataService.CountJsonFilesForDirectory(JsonDataService.CatalogueDirectory) < catalogueSelection.value)
        {
            print("ERROR [DropdownManager.cs.SetContents_QuestionAnswerButtons()]: Invalid Index for Fragenkatalognummer: " + catalogueSelection.value);
            return;
        }
        if (dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{Global.CurrentQuestionRound.catalogueIndex}.json").questions.Count < questionSelection.value)
        {
            print("ERROR [DropdownManager.cs.SetContents_QuestionAnswerButtons()]: Invalid Index for Fragennummer: " + catalogueSelection.value + " in Fragenkatalognummer: " + questionSelection.value);
            return;
        }
        Question currentQuestion = dataService.LoadData<Catalogue>(JsonDataService.CatalogueDirectory + $"/{catalogueSelection.value}.json").questions[questionSelection.value];
        QButton_Label.text = currentQuestion.text;
        AButton1_Label.text = currentQuestion.answers[0].text;
        AButton1_Label.text = currentQuestion.answers[0].text;
        AButton2_Label.text = currentQuestion.answers[1].text;
        AButton3_Label.text = currentQuestion.answers[2].text;
        AButton4_Label.text = currentQuestion.answers[3].text;
    }

}

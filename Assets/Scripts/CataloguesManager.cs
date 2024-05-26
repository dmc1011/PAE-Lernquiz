using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CataloguesManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown Katalogauswahl;
    [SerializeField] private TMP_Dropdown Fragenauswahl;
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

    void Start()
    {
        if(SceneManager.GetActiveScene().name != "Screen_Catalogues")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
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
        KatalogauswahlChangedEvent(); // This will update Fragenauswahl
    }

    public void KatalogauswahlChangedEvent()
    {
        Fragenauswahl.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        if (DataManager.Storage.Catalogues.Count != 0)
        {
            for (int i = 0; i < DataManager.Storage.Catalogues[Katalogauswahl.value].questions.Count; i++)
            {
                options.Add(new(DataManager.Storage.Catalogues[Katalogauswahl.value].questions[i].question.text));
            }
        }
        if (options.Count == 0)
        {
            options.Add(new("Nicht verfügbar"));
        }
        Fragenauswahl.AddOptions(options);
        FragenauswahlChangedEvent();
    }

    public void FragenauswahlChangedEvent()
    {
        if (DataManager.Storage.Catalogues.Count == 0)
        {
            print("ERROR [CataloguesManager.cs:FragenauswahlChangedEvent()]: Wir benötigen mehr Fragenkataloge, Milord.");
            return;
        }
        if (DataManager.Storage.Catalogues.Count < Katalogauswahl.value)
        {
            print("ERROR [DropdownManager.cs.SetContents_QuestionAnswerButtons()]: Invalid Index for Fragenkatalognummer: " + Katalogauswahl.value);
            return;
        }
        if (DataManager.Storage.Catalogues[Katalogauswahl.value].questions.Count < Fragenauswahl.value)
        {
            print("ERROR [DropdownManager.cs.SetContents_QuestionAnswerButtons()]: Invalid Index for Fragennummer: " + Katalogauswahl.value + " in Fragenkatalognummer: " + Fragenauswahl.value);
            return;
        }
        DataManager.Question currentQuestion = DataManager.Storage.Catalogues[Katalogauswahl.value].questions[Fragenauswahl.value];
        QButton_Label.text = currentQuestion.question.text;
        AButton1_Label.text = currentQuestion.answers[0].text;
        AButton2_Label.text = currentQuestion.answers[1].text;
        AButton3_Label.text = currentQuestion.answers[2].text;
        AButton4_Label.text = currentQuestion.answers[3].text;
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DropdownManager : MonoBehaviour
{

    [SerializeField] private string type; // will be set in Editor
    [SerializeField] private TMP_Dropdown other_dropdown;
    [SerializeField] private UnityEngine.UI.Button QButton;
    [SerializeField] private UnityEngine.UI.Button AButton1;
    [SerializeField] private UnityEngine.UI.Button AButton2;
    [SerializeField] private UnityEngine.UI.Button AButton3;
    [SerializeField] private UnityEngine.UI.Button AButton4;

    private TMP_Dropdown me;
    private RectTransform rectTransform;

    void Start()
    {
        me = GetComponent<TMP_Dropdown>();
        rectTransform = me.transform.GetComponent<RectTransform>();
        SetContents();
        SetDesign();
    }

    public void ChangeEvent()
    {
        print(me.value);

        switch (type)
        {
            // Der ausgewählte Fragenkatalog wurde geändert.
            case "Fragenkataloge_Fragenkataloge": {
                other_dropdown.ClearOptions();
                List<TMP_Dropdown.OptionData> options = new();
                for (int i = 0; i < Global.CatalogueStorage.catalogues[me.value].questions.Count; i++)
                {
                    options.Add(new(Global.CatalogueStorage.catalogues[me.value].questions[i].question.text));
                }
                other_dropdown.AddOptions(options);
            } break;

            case "Fragenkataloge_Fragen": {
                SetContents_QuestionAnswerButtons();
            } break;

        }

    }

    private void SetContents_QuestionAnswerButtons()
    {
        if (Global.CatalogueStorage.catalogues.Count == 0) return;
        if (Global.CatalogueStorage.catalogues.Count < other_dropdown.value)
        {
            print("ERROR: Invalid Index for Fragenkatalognummer");
            return;
        }
        if (Global.CatalogueStorage.catalogues[other_dropdown.value].questions.Count < me.value)
        {
            print("ERROR: Invalid Index for Fragennummer im Fragenkatalog " + other_dropdown.value);
            return;
        }
        Global.Question currentQuestion = Global.CatalogueStorage.catalogues[other_dropdown.value].questions[me.value];
        QButton.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.question.text;
        AButton1.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[0].text;
        AButton2.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[1].text;
        AButton3.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[2].text;
        AButton4.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[3].text;
    }

    private void SetContents()
    {
        switch (type) {

            case "Fragenkataloge_Fragenkataloge": {
                    me.ClearOptions();
                    List<TMP_Dropdown.OptionData> options = new();
                    for (int i = 0; i < Global.CatalogueStorage.catalogues.Count; i++)
                    {
                        options.Add(new(Global.CatalogueStorage.catalogues[i].name));
                    }
                    me.AddOptions(options);
            } break;

            case "Fragenkataloge_Fragen": {
                    if (Global.CatalogueStorage.catalogues.Count == 0) break;
                    me.ClearOptions();
                    List<TMP_Dropdown.OptionData> options = new();
                    for (int i = 0; i < Global.CatalogueStorage.catalogues[other_dropdown.value].questions.Count; i++)
                    {
                        options.Add(new(Global.CatalogueStorage.catalogues[other_dropdown.value].questions[i].question.text));
                    }
                    me.AddOptions(options);
                    SetContents_QuestionAnswerButtons();
            } break;

        }

    }

    // Hier wird das Design ALLER Dropdowns bestimmt.
    private void SetDesign()
    {
        ColorBlock colors = me.colors;

        // Das würde man normalerweise (wenn es "nur" Farben wären) nicht so
        // aufspalten sondern ohne redundanz schreiben. Falls noch was dazukommt aber lieber so.
        switch (type) {

            case "Fragenkataloge_Fragenkataloge": {
                colors.normalColor = Global.Colors.ButtonBack.Normal;
                colors.pressedColor = Global.Colors.ButtonBack.Pressed;
                colors.highlightedColor = Global.Colors.ButtonBack.Hover;
                me.colors = colors;
                SetPos(0.5f, 0.90f, 0.7f, 0.075f); // Der Zurück Button ist immer unten links.
            } return;


            case "Fragenkataloge_Fragen": {
                colors.normalColor = Global.Colors.ButtonBack.Normal;
                colors.pressedColor = Global.Colors.ButtonBack.Pressed;
                colors.highlightedColor = Global.Colors.ButtonBack.Hover;
                me.colors = colors;
                SetPos(0.5f, 0.82f, 0.7f, 0.075f); // Der Zurück Button ist immer unten links.
            } return;
        }

        print("ERROR in \"DropdownManager.cs\" -> SetDesign(): DropDown has no type.");
    }

    private void SetPos(float x, float y, float w, float h)
    {
        // MS:
        // x, y - Relative Bildschirmposition für das CENTER des Objekts
        // w, h - Prozentsatz der Bildschirmbreite & Höhe
        // Koordinaten:
        // 0/1 ------- 1/1
        //  |           |
        //  |           |
        //  |           |
        //  |  0.5/0.5  |
        //  |           |
        //  |           |
        //  |           |
        // 0/0 ------- 1/0
        rectTransform.offsetMin = new Vector2(-w / 2.0f * Global.width, -h / 2.0f * Global.height);
        rectTransform.offsetMax = new Vector2(w / 2.0f * Global.width, h / 2.0f * Global.height);
        rectTransform.anchoredPosition = new Vector2((x - 0.5f) * Global.width, (y - 0.5f) * Global.height);
    }

}

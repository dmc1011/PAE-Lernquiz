using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private string type; // will be set in Editor
    private Button me; // set this to "self"
    private RectTransform rectTransform;

    void Start()
    {
        // Assign the same colors to every Button
        me = GetComponent<Button>();
        rectTransform = me.transform.GetComponent<RectTransform>();
        SetDesign();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ClickEvent()
    {
        print("Debug: Click Event fired");
        switch (type)
        {
            case "DummyDataLoader":

                Global.CatalogueStorage.Clear();
                for (int i = 0; i < 5; i++)
                {
                    Global.Catalogue catalogue = new("");
                    string z = Random.Range(100, 999).ToString();
                    catalogue.id = "Dummy Katalog ID: " + z;
                    catalogue.name = "Dummy Katalog: " + z;
                    catalogue.ownerId = "Ich";
                    for (int i_question = 0; i_question < 5; i_question++)
                    {
                        Global.Question q = new Global.Question("");
                        q.id = "some question identifier: " + Random.Range(100, 999).ToString();
                        z = Random.Range(100, 999).ToString();
                        q.question = new("Frage mit Zufallszahl: " + z);
                        List<Global.ImageOrText> answers = new() {
                            new(z),
                            new(Random.Range(100, 999).ToString()),
                            new(Random.Range(100, 999).ToString()),
                            new(Random.Range(100, 999).ToString())
                        };
                        q.answers = answers;
                        q.correctAnswerIndex = 0; // erste ist immer richtig -> index 0
                        catalogue.questions.Add(q);
                    }
                    Global.CatalogueStorage.catalogues.Add(catalogue);
                }
                //print(Global.CatalogueStorage);
                break;
        }
    }

    // Hier wird das Design ALLER Buttons bestimmt.
    private void SetDesign()
    {
        

        // Das würde man normalerweise (wenn es "nur" Farben wären) nicht so
        // aufspalten sondern ohne redundanz schreiben. Falls noch was dazukommt aber lieber so.
        switch (type) {

            case "Back": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonBack.Normal;
                colors.pressedColor = Global.Colors.ButtonBack.Pressed;
                colors.highlightedColor = Global.Colors.ButtonBack.Hover;
                me.colors = colors;
                SetPos(0.15f, 0.075f, 0.15f, 0.075f); // Der Zurück Button ist immer unten links.
            } return;

            case "Navigation": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonNavigation.Normal;
                colors.pressedColor = Global.Colors.ButtonNavigation.Pressed;
                colors.highlightedColor = Global.Colors.ButtonNavigation.Hover;
                me.colors = colors;
            } return;

            case "Question": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonQuestion.Normal;
                colors.pressedColor = Global.Colors.ButtonQuestion.Pressed;
                colors.highlightedColor = Global.Colors.ButtonQuestion.Hover;
                me.colors = colors;
            } return;

            case "Fragenkataloge_Frage": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonQuestion.Normal;
                colors.pressedColor = Global.Colors.ButtonQuestion.Pressed;
                colors.highlightedColor = Global.Colors.ButtonQuestion.Hover;
                me.colors = colors;
                SetPos(0.5f, 0.65f, 0.8f, 0.15f);
            } return;

            case "Fragenkataloge_Antwort1": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonAnswer.Normal;
                colors.pressedColor = Global.Colors.ButtonAnswer.Pressed;
                colors.highlightedColor = Global.Colors.ButtonAnswer.Hover;
                me.colors = colors;
                SetPos(0.5f - 0.2f, 0.425f - 0.075f, 0.4f, 0.15f);
            } return;

            case "Fragenkataloge_Antwort2": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonAnswer.Normal;
                colors.pressedColor = Global.Colors.ButtonAnswer.Pressed;
                colors.highlightedColor = Global.Colors.ButtonAnswer.Hover;
                me.colors = colors;
                SetPos(0.5f - 0.2f, 0.425f + 0.075f, 0.4f, 0.15f);
            } return;

            case "Fragenkataloge_Antwort3": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonAnswer.Normal;
                colors.pressedColor = Global.Colors.ButtonAnswer.Pressed;
                colors.highlightedColor = Global.Colors.ButtonAnswer.Hover;
                me.colors = colors;
                SetPos(0.5f + 0.2f, 0.425f - 0.075f, 0.4f, 0.15f);
            } return;

            case "Fragenkataloge_Antwort4": {
                ColorBlock colors = me.colors;
                colors.normalColor = Global.Colors.ButtonAnswer.Normal;
                colors.pressedColor = Global.Colors.ButtonAnswer.Pressed;
                colors.highlightedColor = Global.Colors.ButtonAnswer.Hover;
                me.colors = colors;
                SetPos(0.5f + 0.2f, 0.425f + 0.075f, 0.4f, 0.15f);
            } return;

        }
        print("ERROR in \"ButtonManager.cs\" -> SetDesign(): Button has no type.");
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

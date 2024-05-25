using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class ButtonManager : MonoBehaviour
{

    public string buttonType; // will be set in Editor
    private Button btn; // set this to "self"
    private RectTransform rectTransform;

    void Start()
    {
        // Assign the same colors to every Button
        btn = GetComponent<Button>();
        rectTransform = btn.transform.GetComponent<RectTransform>();
        SetDesign();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Hier wird das Design ALLER Buttons bestimmt.
    // Buttons sind durch "BUTTON_TYPE" voneinander unterscheidbar.
    private void SetDesign()
    {
        ColorBlock colors = btn.colors;

        // Das würde man normalerweise (wenn es "nur" Farben wären) nicht so
        // aufspalten sondern ohne redundanz schreiben. Falls noch was dazukommt aber lieber so.
        switch (buttonType) {
            case "Back":
                colors.normalColor = Global.Colors.ButtonBack.Normal;
                colors.pressedColor = Global.Colors.ButtonBack.Pressed;
                colors.highlightedColor = Global.Colors.ButtonBack.Hover;
                btn.colors = colors;
                SetPos(0.15f, 0.075f, 0.15f, 0.075f); // Der Zurück Button ist immer unten links.

                return;
            case "Navigation":
                colors.normalColor = Global.Colors.ButtonNavigation.Normal;
                colors.pressedColor = Global.Colors.ButtonNavigation.Pressed;
                colors.highlightedColor = Global.Colors.ButtonNavigation.Hover;
                btn.colors = colors;


                return;
            case "Question":
                colors.normalColor = Global.Colors.ButtonQuestion.Normal;
                colors.pressedColor = Global.Colors.ButtonQuestion.Pressed;
                colors.highlightedColor = Global.Colors.ButtonQuestion.Hover;
                btn.colors = colors;

                return;
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

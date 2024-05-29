using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LabelManager : MonoBehaviour
{
    private string scene;
    private TextMeshProUGUI me;
    private RectTransform rectTransform;

    void Start()
    {
        me = GetComponent<TextMeshProUGUI>();
        rectTransform = me.transform.GetComponent<RectTransform>();
        scene = SceneManager.GetActiveScene().name;
        SetDesign();
        SetContents();
    }

    // Generic SetContents. Use this if no other Scripts handle Label Initialization.
    private void SetContents()
    {
        switch(scene)
        {
            case "Screen_SingleplayerGameloop_1": // Wird von SingleplayerGameloop1Manager.cs verwaltet.
                { }
                break;

            default:
                {
                    GenericError("SetContents");
                }
                break;
        }

    }

    // SetDesign
    // Damit alle UI-Elemente Flächendeckend denselben Stil haben werden ALLE LABELS hier behandelt.
    // Das funcktioniert nur, wenn die UI-Elemente im Unity-Editor den selben Namen verwenden wie hier.
    private void SetDesign()
    {
        switch (scene)
        {
            case "Screen_SingleplayerGameloop_1":
                {
                    switch (name)
                    {
                        case "Fragenummer":
                            {
                                me.color = UIDesign.Colors.Labels.Fragenummer;
                            }
                            break;

                        default:
                            {
                                GenericError("SetDesign");
                            }
                            break;
                    }

                }
                break;

            default:
                {
                    GenericError("SetDesign");
                }
                break;
        }
    }

    private void GenericError(string func)
    {
        print("ERROR [LabelManager.cs." + func + "()]: Name of Label is \"" + name + "\" & Scene is \"" + scene + "\" -> unknown combination.");
    }

    private void SetPos(Rect rect)
    {
        SetPos(rect.x, rect.y, rect.width, rect.height);
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
        rectTransform.offsetMin = new Vector2(-w / 2.0f * UIDesign.Positions.Global.width, -h / 2.0f * UIDesign.Positions.Global.height);
        rectTransform.offsetMax = new Vector2(w / 2.0f * UIDesign.Positions.Global.width, h / 2.0f * UIDesign.Positions.Global.height);
        rectTransform.anchoredPosition = new Vector2((x - 0.5f) * UIDesign.Positions.Global.width, (y - 0.5f) * UIDesign.Positions.Global.height);
    }
}

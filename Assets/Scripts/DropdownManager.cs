using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    private string scene;
    private TMP_Dropdown me;
    private RectTransform rectTransform;
    [SerializeField] private Button SomeButton; // TODO MS

    void Start()
    {
        me = GetComponent<TMP_Dropdown>();
        rectTransform = me.transform.GetComponent<RectTransform>();
        scene = SceneManager.GetActiveScene().name;
        SetDesign();
        SetContents();
    }

    // Generic Change event. Use this if no other Scripts handle Dropdown changes.
    public void ChangeEvent()
    {
        switch (scene)
        {
            case "Screen_Catalogues": // Wird von CataloguesManager.cs behandelt.
                { }
                break;

            case "Screen_NewGame": // Hier braucht man keinen eigenen Manager.
                {
                    switch (name)
                    {
                        case "Katalogauswahl":
                            {
                                Global.AktuelleFragerunde.CatalogueIndex = me.value;
                            }
                            break;

                        default:
                            {
                                GenericError("ChangeEvent");
                            }
                            break;
                    }

                } break;

            default:
                {
                    GenericError("ChangeEvent");
                }
                break;

        }

    }

    // Generic SetContents. Use this if no other Scripts handle Dropdown Initialization.
    private void SetContents()
    {
        switch (scene)
        {

            case "Screen_NewGame":
            case "Screen_Catalogues": // Wird von CataloguesManager.cs behandelt.
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
    // Damit alle UI-Elemente Flächendeckend denselben Stil haben werden ALLE DROPDOWNS hier behandelt.
    // Das funcktioniert nur, wenn die UI-Elemente im Unity-Editor den selben Namen verwenden wie hier.
    private void SetDesign()
    {
        switch (name)
        {

            case "Katalogauswahl":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Dropdowns.Katalogauswahl.Normal;
                    colors.pressedColor = UIDesign.Colors.Dropdowns.Katalogauswahl.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Dropdowns.Katalogauswahl.Hover;
                    me.colors = colors;

                    switch (scene)
                    {
                        case "Screen_Catalogues":
                            {
                                SetPos(UIDesign.Positions.Dropdowns.Catalogues.Katalogauswahl);
                            }
                            break;

                        case "Screen_NewGame":
                            {
                                SetPos(UIDesign.Positions.Dropdowns.NewGame.Katalogauswahl);
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

            case "Fragenauswahl":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Dropdowns.Fragenauswahl.Normal;
                    colors.pressedColor = UIDesign.Colors.Dropdowns.Fragenauswahl.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Dropdowns.Fragenauswahl.Hover;
                    me.colors = colors;

                    switch (scene)
                    {
                        case "Screen_Catalogues":
                            {
                                SetPos(UIDesign.Positions.Dropdowns.Catalogues.Fragenauswahl);
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
        print("ERROR [DropdownManager.cs." + func + "()]: Name of Dropdown is \"" + name + "\" & Scene is \"" + scene + "\" -> unknown combination.");
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

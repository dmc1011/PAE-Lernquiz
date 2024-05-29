using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class ButtonManager : MonoBehaviour
{
    private string scene; // this is the name of the current scene.
    private Button me; // set this to "self"
    private RectTransform rectTransform;

    void Start()
    {
        me = GetComponent<Button>();
        rectTransform = me.transform.GetComponent<RectTransform>();
        scene = SceneManager.GetActiveScene().name;
        SetDesign();
    }

    // Für die Navigation. ClickEvent hier drauf legen -> Neue Szene laden.
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Generic Change event. Use this if no other Scripts handle Dropdown changes.
    public void ClickEvent()
    {

        switch (scene)
        {
            case "Screen_Home":
                {
                    switch (name)
                    {
                        case "DummyDataLoader":
                            {
                                DataManager.LoadDummyData();
                            }
                            break;

                        default:
                            {
                                GenericError("ClickEvent");
                            }
                            break;
                    }

                }
                break;

            default:
                {
                    GenericError("ClickEvent");
                }
                break;
        }


    }

    // SetDesign
    // Damit alle UI-Elemente Flächendeckend denselben Stil haben werden ALLE BUTTONS hier behandelt.
    // (Einzige Ausnahme ist der SingleplayerGameloop, da hier das verschieben der Buttons zum Randomisieren verwendet wird)
    // Das funcktioniert nur, wenn die UI-Elemente im Unity-Editor den selben Namen verwenden wie hier.
    private void SetDesign()
    {
        switch (name)
        {
            case "DummyDataLoader":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Back.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Back.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Back.Hover;
                    me.colors = colors;
                }
                break;

            case "Back":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Back.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Back.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Back.Hover;
                    me.colors = colors;
                    SetPos(UIDesign.Positions.Buttons.Back);
                }
                break;

            case "Weiter":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Navigation.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Navigation.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Navigation.Hover;
                    me.colors = colors;
                    print("TODO: the \"" + name + "\" button is not fine yet.");
                }
                break;

            case "StartLineareRunde":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Navigation.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Navigation.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Navigation.Hover;
                    me.colors = colors;
                    SetPos(UIDesign.Positions.Buttons.NewGame.StartLineareRunde);
                } break;

            case "StartZufallsrunde":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Navigation.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Navigation.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Navigation.Hover;
                    me.colors = colors;
                    SetPos(UIDesign.Positions.Buttons.NewGame.StartZufallsrunde);
                }
                break;

            case "Start": case "Settings": case "DailyTask": case "Questions":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Navigation.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Navigation.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Navigation.Hover;
                    me.colors = colors;

                    switch (name)
                    {
                        case "Start":
                            {
                                switch(scene)
                                {
                                    case "Screen_Home":
                                        {
                                            SetPos(UIDesign.Positions.Buttons.Home.Start);
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Settings":
                            {
                                SetPos(UIDesign.Positions.Buttons.Home.Settings);
                            }
                            break;
                        case "DailyTask":
                            {
                                SetPos(UIDesign.Positions.Buttons.Home.DailyTask);
                            }
                            break;
                        case "Questions":
                            {
                                SetPos(UIDesign.Positions.Buttons.Home.Questions);
                            }
                            break;
                    }

                }
                break;

            case "Frage":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Question.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Question.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Question.Hover;
                    me.colors = colors;
                    switch (scene)
                    {
                        case "Screen_Catalogues":
                            {
                                SetPos(UIDesign.Positions.Buttons.Catalogues.Frage);
                            }
                            break;

                        case "Screen_SingleplayerGameloop_1": // Wird von SingleplayerGameloop1Manager.cs verwaltet
                            { }
                            break;

                        default:
                            {
                                GenericError("SetDesign");
                            }
                            break;
                    }
                }
                break;

            case "Antwort1": case "Antwort2": case "Antwort3": case "Antwort4":
                {
                    ColorBlock colors = me.colors;
                    colors.normalColor = UIDesign.Colors.Buttons.Answer.Normal;
                    colors.pressedColor = UIDesign.Colors.Buttons.Answer.Pressed;
                    colors.highlightedColor = UIDesign.Colors.Buttons.Answer.Hover;
                    me.colors = colors;


                    switch (name)
                    {
                        case "Antwort1":
                            {
                                switch (scene)
                                {
                                    case "Screen_Catalogues":
                                        {
                                            SetPos(UIDesign.Positions.Buttons.Catalogues.Antwort1);
                                        }
                                        break;

                                    case "Screen_SingleplayerGameloop_1": // Wird von SingleplayerGameloop1Manager.cs verwaltet
                                        { }
                                        break;

                                    default:
                                        {
                                            GenericError("SetDesign");
                                        }
                                        break;
                                }
                            }
                            break;

                        case "Antwort2":
                            {
                                switch (scene)
                                {
                                    case "Screen_Catalogues":
                                        {
                                            SetPos(UIDesign.Positions.Buttons.Catalogues.Antwort2);
                                        }
                                        break;

                                    case "Screen_SingleplayerGameloop_1": // Wird von SingleplayerGameloop1Manager.cs verwaltet
                                        { }
                                        break;

                                    default:
                                        {
                                            GenericError("SetDesign");
                                        }
                                        break;
                                }
                            }
                            break;

                        case "Antwort3":
                            {
                                switch (scene)
                                {
                                    case "Screen_Catalogues":
                                        {
                                            SetPos(UIDesign.Positions.Buttons.Catalogues.Antwort3);
                                        }
                                        break;

                                    case "Screen_SingleplayerGameloop_1": // Wird von SingleplayerGameloop1Manager.cs verwaltet
                                        { }
                                        break;

                                    default:
                                        {
                                            GenericError("SetDesign");
                                        }
                                        break;
                                }
                            }
                            break;

                        case "Antwort4":
                            {
                                switch (scene)
                                {
                                    case "Screen_Catalogues":
                                        {
                                            SetPos(UIDesign.Positions.Buttons.Catalogues.Antwort4);
                                        }
                                        break;

                                    case "Screen_SingleplayerGameloop_1": // Wird von SingleplayerGameloop1Manager.cs verwaltet
                                        { }
                                        break;

                                    default:
                                        {
                                            GenericError("SetDesign");
                                        }
                                        break;
                                }
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
        print("ERROR [ButtonManager.cs." + func + "()]: Name of Button is \"" + name + "\" & Scene is \"" + scene + "\" -> unknown combination.");
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

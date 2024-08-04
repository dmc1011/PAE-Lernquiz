using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarConfig : MonoBehaviour
{

    [SerializeField] private bool scale = false;
    [SerializeField] private Sprite style;
    [SerializeField] private CanvasRenderer bar1;
    [SerializeField] private CanvasRenderer bar2;
    [SerializeField] private CanvasRenderer bar3;
    [SerializeField] private CanvasRenderer background;
    [SerializeField] private CanvasRenderer border;
    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;
    [SerializeField] private Slider slider3;
    [SerializeField] private Color color1 = Color.black;
    [SerializeField] private Color color2 = Color.green;
    [SerializeField] private Color color3 = Color.red;
    [SerializeField] private Color colorBackground = Color.white;
    [SerializeField] private Color colorBorder = Color.black;
    [SerializeField, Range(0, 1)] private float value1 = 1.0f;
    [SerializeField, Range(0, 1)] private float value2 = 1.0f;
    [SerializeField, Range(0, 1)] private float value3 = 1.0f;

    void Start()
    {
        bar1.GetComponent<Image>().color = color1;
        bar2.GetComponent<Image>().color = color2;
        bar3.GetComponent<Image>().color = color3;
        background.GetComponent<Image>().color = colorBackground;
        border.GetComponent<Image>().color = colorBorder;

        bar1.GetComponent<Image>().sprite = style;
        bar2.GetComponent<Image>().sprite = style;
        bar3.GetComponent<Image>().sprite = style;
        background.GetComponent<Image>().sprite = style;
        border.GetComponent<Image>().sprite = style;
    }

    // Just for usage in the editor
    public void OnValidate()
    {
        SetValue(value1, value2, value3);
        Start();
    }

    public void SetValue(float a, float b, float c)
    {
        value1 = a > 0 ? a : 0;
        value2 = b > 0 ? b : 0;
        value3 = c > 0 ? c : 0;

        if (scale)
        {
            float max = value1;
            if (value2 > max) max = value2;
            if (value3 > max) max = value3;

            slider1.value = value1 / max;
            slider2.value = value2 / max;
            slider3.value = value3 / max;
        }
        else
        {
            float sum = value1 + value2 + value3;

            slider1.value = value1 / sum;
            slider2.value = value2 / sum;
            slider3.value = value3 / sum;
        }
    }

}

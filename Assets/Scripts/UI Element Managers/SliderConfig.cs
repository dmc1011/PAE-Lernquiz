using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderConfig : MonoBehaviour
{

    [SerializeField] private CanvasRenderer circle1;
    [SerializeField] private CanvasRenderer circle2;
    [SerializeField] private CanvasRenderer circle3;
    [SerializeField] private CanvasRenderer circleCenter;
    [SerializeField] private TextMeshProUGUI label; 
    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;
    [SerializeField] private Color color1 = Color.black;
    [SerializeField] private Color color2 = Color.green;
    [SerializeField] private Color color3 = Color.red;
    [SerializeField] private Color colorText = Color.white;
    [SerializeField] private Color colorCenter = Color.gray;
    [SerializeField, Range(0, 5)] private float value1 = 1.0f;
    [SerializeField, Range(0, 5)] private float value2 = 1.0f;
    [SerializeField, Range(0, 5)] private float value3 = 1.0f;

    void Start()
    {
        circle1.GetComponent<Image>().color = color1;
        circle2.GetComponent<Image>().color = color2;
        circle3.GetComponent<Image>().color = color3;
        circleCenter.GetComponent<Image>().color = colorCenter;
        label.color = colorText;
    }

    // Just for usage in the editor
    public void OnValidate()
    {
        SetValue(value1, value2, value3);
    }

    public void SetValue(float a, float b, float c)
    {
        value1 = a > 0 ? a : 0;
        value2 = b > 0 ? b : 0;
        value3 = c > 0 ? c : 0;
        float sum = value1 + value2 + value3;
        slider1.value = value1 / sum;
        slider2.value = (value1 + value2) / sum;
    }

    public void SetLabel(string text)
    {
        label.text = text;
    }


}

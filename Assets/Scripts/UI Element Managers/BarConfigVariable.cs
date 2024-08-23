using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BarConfigVariable : MonoBehaviour
{
    public enum Modes
    {
        sum = 0,
        max = 1,
        direct = 2,
    }

    private int numberOfBars = 1;
    [SerializeField] private Modes scale = 0;
    [SerializeField] private Sprite style;
    [SerializeField] private CanvasRenderer background;
    [SerializeField] private CanvasRenderer border;
    [SerializeField] private GameObject sliderTemplate;
    [SerializeField] private List<GameObject> sliders = new List<GameObject>();
    [SerializeField] private List<Color> colors = new List<Color>();
    [SerializeField] private Color colorBackground = Color.white;
    [SerializeField] private Color colorBorder = Color.black;
    [SerializeField, Range(0, 100)] private List<float> values = new List<float>();

    void Awake()
    {
        for (int i = 0; i < numberOfBars; i++)
        {
            sliders[i] = Instantiate(sliderTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            sliders[i].transform.SetParent(this.transform, false);

            sliders[i].transform.GetChild(0).GetComponent<Image>().color = colors[0];
            sliders[i].transform.GetChild(0).GetComponent<Image>().sprite = style;
        }

        background.GetComponent<Image>().color = colorBackground;
        background.GetComponent<Image>().sprite = style;

        border.GetComponent<Image>().color = colorBorder;
        border.GetComponent<Image>().sprite = style;
    }

    public void Update()
    {
        
    }

    public void OnValidate()
    {
        // SetNumberOfBars(numberOfBars);
        // SetValue(values);
        // SetColor(colors);
        // SetScale(scale);
    }

    private void AdjustLists()
    {
        for (int i = 0; colors.Count < sliders.Count; i++)
        {
            colors.Add(Color.black);
        }

        for (int i = 0; colors.Count > sliders.Count; i++)
        {
            colors.RemoveAt(colors.Count - 1);
        }

        for (int i = 0; values.Count < sliders.Count; i++)
        {
            values.Add(1.0f);
        }

        for (int i = 0; values.Count > sliders.Count; i++)
        {
            values.RemoveAt(values.Count - 1);
        }
    }

    public void AdjustPositions()
    {
        for (int i = 0; i < numberOfBars; i++)
        {
            sliders[i].transform.localPosition = new Vector3(0, -i * 90/numberOfBars - 5, 0);
        }
    }

    public void AdjustBarHeight()
    {
        for (int i = 0; i < numberOfBars; i++)
        {
            sliders[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 90/numberOfBars);
        }
    }

    public void SetScale(Modes scale)
    {
        this.scale = scale;

        SetValue(values);
    }

    public void SetValue(List<float> values)
    {
        this.values = values;

        float sum = 0.0f;
        float max = values[0];

        foreach (float value in values)
        {
            sum += value;
            if (value > max) max = value;
        }

        for (int i = 0; i < numberOfBars; i++)
        {
            if (scale == Modes.max)
            {
                sliders[i].GetComponent<Slider>().value = values[i] / max;
            }
            else if (scale == Modes.sum)
            {
                sliders[i].GetComponent<Slider>().value = values[i] / sum;
            }
            else if (scale == Modes.direct)
            {
                sliders[i].GetComponent<Slider>().value = values[i] / 100.0f;
            }
        }
    }

    public void SetColor(List<Color> colors)
    {
        this.colors = colors;
        for (int i = 0; i < numberOfBars; i++)
        {
            sliders[i].transform.GetChild(0).GetComponent<Image>().color = colors[i];
        }
    }

    public void SetNumberOfBars(int numberOfBars)
    {
        for (int i = this.numberOfBars; i < numberOfBars; i++)
        {
            sliders.Add(Instantiate(sliderTemplate, new Vector3(0, 0, 0), Quaternion.identity));
            sliders[i].transform.SetParent(this.transform, false);

            sliders[i].transform.GetChild(0).GetComponent<Image>().color = colors[0];
            sliders[i].transform.GetChild(0).GetComponent<Image>().sprite = style;
        }

        this.numberOfBars = numberOfBars;

        AdjustLists();
        AdjustPositions();
        AdjustBarHeight();
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationButton : MonoBehaviour
{

    [SerializeField] private Image colorDisplay;
    [SerializeField] private TextMeshProUGUI numberLabel;
    [SerializeField] private TextMeshProUGUI textLabel;

    [SerializeField] private Color correctColor;
    [SerializeField] private Color wrongColor;

    public void Set(int number, bool correct, string text)
    {
        colorDisplay.color = correct ? correctColor : wrongColor;
        numberLabel.text = number.ToString();
        textLabel.text = text;
    }
}

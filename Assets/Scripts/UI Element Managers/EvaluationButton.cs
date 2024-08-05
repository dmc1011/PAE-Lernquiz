using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationButton : MonoBehaviour
{

    [SerializeField] private GameObject colorDisplayWrong;
    [SerializeField] private GameObject colorDisplayCorrect;
    [SerializeField] private TextMeshProUGUI numberLabel;
    [SerializeField] private TextMeshProUGUI textLabel;

    public void Set(int number, bool correct, string text)
    {
        colorDisplayCorrect.SetActive(correct);
        colorDisplayWrong.SetActive(!correct);
        numberLabel.text = number.ToString();
        textLabel.text = text;
    }
}

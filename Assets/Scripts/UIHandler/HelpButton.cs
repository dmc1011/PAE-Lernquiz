using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textLabel;

    public void Set(string text)
    {
        textLabel.text = text;
    }
}

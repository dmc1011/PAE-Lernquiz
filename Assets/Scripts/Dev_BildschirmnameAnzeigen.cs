using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dev_BildschirmnameAnzeigen : MonoBehaviour
{
    void Start()
    {
        TextMeshProUGUI label = GetComponent<TextMeshProUGUI>();
        label.text = SceneManager.GetActiveScene().name;
        label.color = Color.red;
        label.fontSize = 14;
    }
}

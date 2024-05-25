using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dev_BildschirmnameAnzeigen : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = SceneManager.GetActiveScene().name;
        GetComponent<TextMeshProUGUI>().color = Color.red;
    }
}

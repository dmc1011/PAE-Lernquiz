using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class ButtonManager : MonoBehaviour
{
    public Button btn;

    void Start()
    {
        // Assign the same colors to every Button
        btn = GetComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = Global.Colors.ButtonNormal;
        colors.pressedColor = Global.Colors.ButtonPressed;
        colors.highlightedColor = Global.Colors.ButtonHover;
        btn.colors = colors;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Global;

public class ButtonNavigation : MonoBehaviour
{
    [SerializeField] private HexagonBackground background = null;
    private string targetSceneName = "";

    public void LoadScene(string sceneName)
    {
        targetSceneName = sceneName;
        if (background != null)
        {
            float timeNeeded = background.TriggerEndSequence();
            Invoke(nameof(LoadSceneInternal), timeNeeded);
        }
        else
        {
            LoadSceneInternal();
        }
    }

    private void LoadSceneInternal()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}

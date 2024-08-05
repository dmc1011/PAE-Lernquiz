using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Global;

public class ButtonNavigation : MonoBehaviour
{
    [SerializeField] private Background bg = null;
    private string targetSceneName = "";

    public void LoadScene(string sceneName)
    {
        targetSceneName = sceneName;
        if (bg != null)
        {
            float timeNeeded = bg.TriggerEndSequence();
            Invoke(nameof(LoadSceneInternal), timeNeeded);
        }
        else
        {
            Debug.Log("Loaded Scene directly");
            LoadSceneInternal();
        }
    }

    private void LoadSceneInternal()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}

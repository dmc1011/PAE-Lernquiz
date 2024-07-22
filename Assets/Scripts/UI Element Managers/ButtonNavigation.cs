using UnityEngine;
using UnityEngine.SceneManagement;

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
            LoadSceneInternal();
        }
    }

    private void LoadSceneInternal()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}

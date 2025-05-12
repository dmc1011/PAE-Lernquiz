using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [SerializeField] private HexagonBackground background;

    private Scene _targetScene;

    public void LoadScene(Scene scene, bool useAnimation = true)
    {
        _targetScene = scene;
        StartSceneTransition(useAnimation);
    }

    public void LoadSceneWithGameMode(Scene scene, GameMode gameMode, bool insideQuestionRound = false, bool useAnimation = true)
    {
        _targetScene = scene;
        Global.SetGameMode(gameMode);
        Global.SetInsideQuestionRound(insideQuestionRound);
        StartSceneTransition(useAnimation);
    }

    private void StartSceneTransition(bool useAnimation = true)
    {
        if (background != null && useAnimation)
        {
            float timeNeeded = background.TriggerEndSequence();
            //System.Threading.Thread.Sleep((int)Mathf.Floor(timeNeeded * 1000));
            Invoke(nameof(LoadSceneInternal), timeNeeded);
            return;
        }

        LoadSceneInternal();
    }

    private void LoadSceneInternal()
    {
        SceneManager.LoadScene((int)_targetScene);
    }
}

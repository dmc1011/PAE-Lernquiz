using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum Scene
{
    Achievements = 0,
    Authentication,
    DailyTask,
    Editor,
    Evaluation,
    Help,
    Home,
    LinearQuiz,
    NewGame,
    PractiseBook,
    Profile,
    RandomQuiz,
    Statistics
}

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private HexagonBackground background;

    private Scene _targetScene;

    public void LoadScene(Scene scene)
    {
        _targetScene = scene;
        StartSceneTransition();
    }

    private void StartSceneTransition(bool useAnimation = true)
    {
        if (background != null && useAnimation)
        {
            float timeNeeded = background.TriggerEndSequence();
            System.Threading.Thread.Sleep((int)Mathf.Floor(timeNeeded * 1000));
            //Invoke(nameof(LoadSceneInternal), timeNeeded);
        }

        LoadSceneInternal();
    }

    private void LoadSceneInternal()
    {
        SceneManager.LoadScene((int)_targetScene);
    }
}

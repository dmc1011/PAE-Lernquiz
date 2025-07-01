using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HomeMenuHandler : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    public void LoadProfileScene()
    {
        sceneLoader.LoadScene(Scene.Profile, false);
    }

    public void LoadHelpScene()
    {
        sceneLoader.LoadScene(Scene.Help, false);
    }

    public void LoadLinearGameSelection()
    {
        sceneLoader.LoadSceneWithGameMode(Scene.ContentSelection, GameMode.LinearQuiz);
    }

    public void LoadRandomGameSelection()
    {
        sceneLoader.LoadSceneWithGameMode(Scene.ContentSelection, GameMode.RandomQuiz);
    }

    public void LoadStatisticsSelection()
    {
        sceneLoader.LoadSceneWithGameMode(Scene.ContentSelection, GameMode.Statistics);
    }

    public void LoadEditorSelection()
    {
        sceneLoader.LoadSceneWithGameMode(Scene.EditorMenu, GameMode.Editor);
    }

    public void LoadPracticeBookSelection()
    {
        sceneLoader.LoadSceneWithGameMode(Scene.ContentSelection, GameMode.PracticeBook);
    }
}

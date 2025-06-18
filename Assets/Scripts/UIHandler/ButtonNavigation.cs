using UnityEngine;

public class ButtonNavigation : MonoBehaviour
{
    [SerializeField] private bool useAnimation;

    [SerializeField] private SceneLoader sceneLoader;

    public void LoadScene(Scene scene)
    {   switch(scene)
        {
            case Scene.Home:
                sceneLoader.LoadSceneWithGameMode(scene, GameMode.None, useAnimation: useAnimation);
                break;
            default:
                sceneLoader.LoadScene(scene);
                break;
        }
    }

    public void LoadHomeFromEvaluation()
    {
        Global.ClearSession();
        sceneLoader.LoadSceneWithGameMode(Scene.Home, GameMode.None, useAnimation: useAnimation);
    }

    public void LoadNewGameFromEvaluation()
    {
        Global.ClearSession();
        sceneLoader.LoadScene(Scene.ContentSelection, useAnimation: useAnimation);
    }

    public void LoadHome()
    {
        Global.ClearSession();
        sceneLoader.LoadSceneWithGameMode(Scene.Home, GameMode.None, useAnimation: useAnimation);
    }
}

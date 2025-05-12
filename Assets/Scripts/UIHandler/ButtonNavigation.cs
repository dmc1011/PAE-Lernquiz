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
}

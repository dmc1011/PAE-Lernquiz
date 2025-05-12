using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonHandler : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    
    public void LoadHomeScene()
    {
        if (sceneLoader != null)
            sceneLoader.LoadSceneWithGameMode(Scene.Home, GameMode.None);
        else
            Debug.Log("Missing Scene Loader");
    }
}

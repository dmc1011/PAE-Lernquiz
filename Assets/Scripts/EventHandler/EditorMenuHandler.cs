using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EditorMenuHandler : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    public void LoadTopicSelection()
    {
        Global.SetEditorType(SceneLoader.EditorType.Topic);
        sceneLoader.LoadSceneWithGameMode(Scene.ContentSelection, GameMode.Editor);
    }

    public void LoadCatalogueSelection()
    {
        Global.SetEditorType(SceneLoader.EditorType.Catalogue);
        sceneLoader.LoadSceneWithGameMode(Scene.ContentSelection, GameMode.Editor);
    }
}

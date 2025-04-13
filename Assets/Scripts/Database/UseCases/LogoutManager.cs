using Controllers;
using Services;
using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;
using TMPro;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using UnityEditor.VisionOS;

public class LogoutManager : MonoBehaviour
{
    private static Client _supabase;
    private UserLoginController _controller = new UserLoginController();

    [SerializeField] private SceneLoader sceneLoader;

    void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();
    }

    public async void PerformSignOut()
    {
        await _controller.SignOut(_supabase);

        sceneLoader.LoadScene(Scene.Authentication);
    }
}

using Controllers;
using Services;
using UnityEngine;
using Client = Supabase.Client;

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

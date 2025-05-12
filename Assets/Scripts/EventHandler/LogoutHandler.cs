using Controllers;
using Repositories;
using Services;
using System;
using UnityEngine;
using UseCases;
using Client = Supabase.Client;

public class LogoutHandler : MonoBehaviour
{
    private static Client _supabase;
    private IUserRepository _userRepo;

    private SignInUseCase _signInUseCase;
    private SignUpUseCase _signUpUseCase;
    private SignOutUseCase _signOutUseCase;
    private CheckSessionUseCase _checkSessionUseCase;

    private UserLoginController _signInController;

    [SerializeField] private SceneLoader sceneLoader;

    void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();
        _userRepo = new SupabaseUserRepository(_supabase);

        _signInUseCase = new SignInUseCase(_userRepo);
        _signUpUseCase = new SignUpUseCase(_userRepo);
        _signOutUseCase = new SignOutUseCase(_userRepo);
        _checkSessionUseCase = new CheckSessionUseCase(_userRepo);

        _signInController = new UserLoginController(_signInUseCase, _signUpUseCase, _signOutUseCase, _checkSessionUseCase);
    }

    public async void PerformSignOut()
    {
        try
        {
            await _signInController.SignOut();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        sceneLoader.LoadScene(Scene.Authentication);
    }
}

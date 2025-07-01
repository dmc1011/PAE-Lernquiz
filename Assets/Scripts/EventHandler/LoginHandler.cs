using Controllers;
using Services;
using Supabase.Gotrue;
using UnityEngine;
using Client = Supabase.Client;
using TMPro;
using System;
using UseCases;
using Repositories;

public class LoginHandler : MonoBehaviour
{
    private static Client _supabase;
    private IUserRepository _userRepo;

    private SignInUseCase _signInUseCase;
    private SignUpUseCase _signUpUseCase;
    private SignOutUseCase _signOutUseCase;
    private CheckSessionUseCase _checkSessionUseCase;

    private UserLoginController _signInController;

    private string _email;
    private string _password;
    private string _name;
    private string _surname;

    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] private TMP_InputField emailInputSignIn;
    [SerializeField] private TMP_InputField passwordInputSignIn;
    [SerializeField] private TextMeshProUGUI signInErrorMessage;

    [SerializeField] private TMP_InputField emailInputSignUp;
    [SerializeField] private TMP_InputField passwordInputSignUp;
    [SerializeField] private TMP_InputField firstName;
    [SerializeField] private TMP_InputField lastName;
    [SerializeField] private TextMeshProUGUI signUpErrorMessage;


    // Start is called before the first frame update
    void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();
        _userRepo = new SupabaseUserRepository(_supabase);

        _signInUseCase = new SignInUseCase(_userRepo);
        _signUpUseCase = new SignUpUseCase(_userRepo);
        _signOutUseCase = new SignOutUseCase(_userRepo);
        _checkSessionUseCase = new CheckSessionUseCase(_userRepo);

        _signInController = new UserLoginController(_signInUseCase, _signUpUseCase, _signOutUseCase, _checkSessionUseCase);

        Debug.Log("Current User: " + _supabase.Auth.CurrentSession?.User?.Id);

        // to do: check if user has logged in before
        // to do: refresh session
    }

    public async void PerformLogin() {
        
        SetSignInErrorMessage("");

        _email = Functions.CleanInput(emailInputSignIn.text);
        _password = Functions.CleanInput(passwordInputSignIn.text);
        
        try
        {
            await _signInController.SignIn(_email, _password);

            if (_signInController.IsSignedIn())
            {
                sceneLoader.LoadScene(Scene.Home);
                // to do: save access tokens
            }
        }
        catch (Exception e) 
        {
            SetSignInErrorMessage(e.Message);
        }
    }

    public async void PerformSignUp()
    {
        SetSignUpErrorMessage("");

        _email = Functions.CleanInput(emailInputSignUp.text);
        _password = Functions.CleanInput(passwordInputSignUp.text);

        _name = Functions.CleanInput(firstName.text);
        _surname = Functions.CleanInput(lastName.text);

        if (_name == Strings.Empty || _name == null || _surname == Strings.Empty || _surname == null)
        {
            SetSignUpErrorMessage("Bitte gib einen Vor- und Nachnamen ein.");
            return;
        }

        try
        {
            await _signInController.SignUp(_email, _password, _name, _surname);

            sceneLoader.LoadScene(Scene.Home);
            // to do: save access tokens
        }
        catch (Exception e)
        {
            SetSignUpErrorMessage(e.Message);
        }
    }

    public void SetSignInErrorMessage(string m)
    {
        signInErrorMessage.text = m;
    }

    public void SetSignUpErrorMessage(string m)
    {
        signUpErrorMessage.text = m;
    }
}

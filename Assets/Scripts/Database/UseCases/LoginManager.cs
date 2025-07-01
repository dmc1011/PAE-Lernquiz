using Controllers;
using Services;
using Supabase.Gotrue;
using UnityEngine;
using Client = Supabase.Client;
using TMPro;
using System;

public class LoginManager : MonoBehaviour
{
    private static Client _supabase;
    private UserLoginController _controller = new UserLoginController();

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
        Debug.Log("Current User: " + _supabase.Auth.CurrentSession?.User?.Id);

        // to do: check if user has logged in before
        // to do: refresh session
    }

    public async void PerformLogin() {
        signInErrorMessage.text = "";

        _email = Functions.CleanInput(emailInputSignIn.text);
        _password = Functions.CleanInput(passwordInputSignIn.text);

        try
        {
            Session session = await _controller.LogIn(_email, _password, _supabase);

            sceneLoader.LoadScene(Scene.Home);
            // to do: save access tokens
        }
        catch (Exception e)
        {
            signInErrorMessage.text = e.Message;
        }
    }

    public async void PerformSignUp()
    {
        signInErrorMessage.text = "";

        _email = Functions.CleanInput(emailInputSignUp.text);
        _password = Functions.CleanInput(passwordInputSignUp.text);

        _name = Functions.CleanInput(firstName.text);
        _surname = Functions.CleanInput(lastName.text);

        if (_name == Strings.Empty || _name == null || _surname == Strings.Empty || _surname == null)
        {
            signUpErrorMessage.text = "Bitte gib einen Vor- und Nachnamen ein.";
            return;
        }

        try
        {
            Models.Profile profile = await _controller.SignUp(_email, _password, _name, _surname, _supabase);

            sceneLoader.LoadScene(Scene.Home);
            // to do: save access tokens
        }
        catch (Exception e)
        {
            signUpErrorMessage.text = e.Message;
        }
    }
}

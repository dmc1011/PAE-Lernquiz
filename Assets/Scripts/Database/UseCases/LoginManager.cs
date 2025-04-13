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



public class LoginManager : MonoBehaviour
{
    private static Client _supabase;
    private UserLoginController _controller = new UserLoginController();

    private string _email;
    private string _password;

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private TextMeshProUGUI emailInputSignIn;
    [SerializeField] private TextMeshProUGUI passwordInputSignIn;
    [SerializeField] private TextMeshProUGUI emailInputSignUp;
    [SerializeField] private TextMeshProUGUI passwordInputSignUp;
    [SerializeField] private TextMeshProUGUI signInErrorMessage;
    [SerializeField] private TextMeshProUGUI signUpErrorMessage;


    // Start is called before the first frame update
    void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();

        // to do: check if user has logged in before
        // to do: refresh session
    }

    public async void PerformLogin() {
        signInErrorMessage.text = "";
        _email = Functions.CleanInput(emailInputSignIn.text);
        _password = Functions.CleanInput(passwordInputSignIn.text);

        Session session = await _controller.LogIn(_email, _password, _supabase);

        if (session?.User != null)
        {
            Debug.Log("Signed In User ID: " + session.User?.Id);

            sceneLoader.LoadScene(Scene.Home);
            // to do: save access tokens
        }
        else
        {
            Debug.Log("Sign in failed");

            signInErrorMessage.text = "Anmeldung fehlgeschlagen.\nÜberprüfe deine Login-Daten.";
            passwordInputSignIn.text = "";
        }
    }

    public async void PerformSignUp()
    {
        signInErrorMessage.text = "";
        _email = Functions.CleanInput(emailInputSignUp.text);
        _password = Functions.CleanInput(passwordInputSignUp.text);

        Session session = await _controller.SignUp(_email, _password, _supabase);

        if (session?.User != null)
        {
            Debug.Log("Signed Up User ID: " + session.User?.Id);

            sceneLoader.LoadScene(Scene.Home);
            // to do: save access tokens
        }
        else
        {
            Debug.Log("Sign up failed");

            signUpErrorMessage.text = "Registrierung fehlgeschlagen.";
            passwordInputSignIn.text = "";
        }
    }
}

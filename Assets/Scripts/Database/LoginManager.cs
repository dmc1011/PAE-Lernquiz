using Controllers;
using Services;
using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;
using TMPro;


public class LoginManager : MonoBehaviour
{
    private static Client _supabase;
    private UserLoginController _controller = new UserLoginController();

    private string _email;
    private string _password;

    [SerializeField] public TextMeshProUGUI emailInputSignIn;
    [SerializeField] public TextMeshProUGUI passwordInputSignIn;
    [SerializeField] public TextMeshProUGUI emailInputSignUp;
    [SerializeField] public TextMeshProUGUI passwordInputSignUp;

    // Start is called before the first frame update
    void Start()
    {
        // initialize client
        _supabase = SupabaseClientProvider.GetClient();

        // to do: check if user has logged in before
        // to do: refresh session
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void PerformLogin() {
        // perform login
        _email = emailInputSignIn.text;
        _password = passwordInputSignIn.text;

        Debug.Log(_email);
        Debug.Log(_password);

        Session session = await _controller.LogIn(_email, _password, _supabase);

        // if login is successful: continue to home menu
        if (session != null)
        {
            Debug.Log("Signed In User ID: " + session?.User?.Id);
            // load next scene - sceneloader object + script, methoden mit scene enum
        }
        else
        {
            Debug.Log("Sign in failed");
            // reset pw text field
            // display info
        }
        Debug.Log(session.ExpiresAt());
    }

    public async void PerformSignUp()
    {
        // perform sign up
        _email = emailInputSignUp.text;
        _password = passwordInputSignUp.text;

        Session session = await _controller.SignUp(_email, _password, _supabase);

        // if sign up is successful: continue to home menu
        if (session != null)
        {
            Debug.Log("Signed Up User ID: " + session?.User?.Id);
            // load next scene - sceneloader object + script, methoden mit scene enum
        }
        else
        {
            Debug.Log("Sign up failed");
            // reset pw text field
            // display info
        }
    }


}

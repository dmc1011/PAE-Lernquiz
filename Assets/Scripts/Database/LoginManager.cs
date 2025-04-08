using Controllers;
using Services;
using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;


public class LoginManager : MonoBehaviour
{
    public const string SUPABASE_URL = Strings.SupabaseUrl;
    public const string SUPABASE_PUBLIC_KEY = Strings.SupabasePublicKey;

    private static Client _supabase;
    private UserLoginController _controller = new UserLoginController();

    private string _email = "";
    private string _password = "";

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
        Session session = await _controller.LogIn(SUPABASE_URL, SUPABASE_PUBLIC_KEY, _supabase);

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
    }

    public async void PerformSignUp()
    {
        // perform sign up
        Session session = await _controller.SignUp(SUPABASE_URL, SUPABASE_PUBLIC_KEY, _supabase);

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

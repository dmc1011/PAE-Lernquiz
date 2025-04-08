using Controllers;
using Services;
using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;

public class SupabaseSetup : MonoBehaviour
{
    public const string SUPABASE_URL = Strings.SupabaseUrl;
    public const string SUPABASE_PUBLIC_KEY = Strings.SupabasePublicKey;

    private static Client _supabase;
    private UserLoginController _controller = new UserLoginController();


    private string email = "dmmccarroll98@gmail.com";
    private string password = "MikeschStevia98";

    // Start is called before the first frame update
    async void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();

        Session session = await _controller.LogIn(email, password, _supabase);
        //Task<Session> signIn = _supabase.Auth.SignIn(email, password);
        //await signIn;
        //Session session = signIn.Result;

        Debug.Log("Signed In User ID: " + session?.User?.Id);
        //Debug.Log(session?.User?.Role);
        //Debug.Log( _supabase.Auth.CurrentUser.UserMetadata);
    }

    public async void HelloWorld()
    {
        var model = new TopicModel
        {
            Name = "Mathe"
        };

        try
        {
            var response = await _supabase.From<TopicModel>().Insert(model);
            Debug.Log("Inserting Database:");
            Debug.Log(response);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

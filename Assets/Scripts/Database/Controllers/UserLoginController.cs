using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;


namespace Controllers
{
    public class UserLoginController
    {

        public async Task<Session> Login(string email, string password, Client client)
        {
            Task<Session> signIn = client.Auth.SignIn(email, password);

            try
            {
                await signIn;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            return signIn.Result;
        }
    }
}

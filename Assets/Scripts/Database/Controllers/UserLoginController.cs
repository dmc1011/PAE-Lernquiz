using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;


namespace Controllers
{
    public class UserLoginController
    {

        public async Task<Session> LogIn(string email, string password, Client client)
        {
            try
            {
                var session = await client.Auth.SignIn(email, password);
                return session;
            }
            catch (GotrueException e)
            {
                Debug.LogError("LogIn-Fehler: " + e.Message);
                return null;
            }
        }

        public async Task<Session> SignUp(string email, string password, Client client)
        {
            try
            {
                var session = await client.Auth.SignUp(email, password);
                return session;
            }
            catch (GotrueException e)
            {
                Debug.LogError("SignUp-Fehler: " + e.Message);
                return null;
            }
        }

        // to do: Log Out
        public async Task SignOut(Client client)
        {
            try
            {
                await client.Auth.SignOut();
            }
            catch (GotrueException e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}

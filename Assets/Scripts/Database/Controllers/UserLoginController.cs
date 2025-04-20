using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System;
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

                if (session?.User == null)
                {
                    throw new SignInException("Anmeldung fehlgeschlagen.\nÜberprüfe deine Login-Daten.");
                }

                return session;
            }
            catch (GotrueException e)
            {
                throw new SignInException("Anmeldung fehlgeschlagen: " + e.Message);
            }
            catch (Exception e)
            {
                throw new SignInException("Unerwarteter Fehler: " + e.Message);
            }
        }

        public async Task<Models.Profile> SignUp(string email, string password, string name, string surname, Client client)

        {
            try
            {
                var session = await client.Auth.SignUp(email, password);

                if (session == null || session.User == null)
                {
                    throw new RegistrationException("Registrierung fehlgeschlagen.");
                }

                // to do: update client (client.Auth.CurrentUser == null)

                var profile = new Models.Profile
                {
                    UserId = Guid.Parse(session.User.Id),
                    Name = name,
                    Surname = surname,
                    Role = UserRole.Student,
                };

                var insert = await client.From<Models.Profile>().Insert(profile);

                return insert.Model;
            }
            catch (GotrueException e)
            {
                throw new RegistrationException("Registrierung fehlgeschlagen: " + e.Message);
            }
            catch (Exception e)
            {
                throw new RegistrationException("Unerwarteter Fehler: " + e.Message); 
            }
        }

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
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}

using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System;
using System.Threading.Tasks;
using Entities;
using DataConversion;
using Client = Supabase.Client;

namespace Repositories
{
    public class SupabaseUserRepository : IUserRepository
    {
        private Client _client;

        public SupabaseUserRepository(Client client)
        {
            _client = client;
        }

        public async Task<Session> SignIn(string email, string password)
        {
            try
            {
                var session = await _client.Auth.SignIn(email, password);

                if (session?.User == null)
                {
                    throw new SignInException("Anmeldung fehlgeschlagen.\nÜberprüfe deine Anmeldedaten.");
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

        public async Task<Profile> SignUp(string email, string password, string name, string surname)
        {
            try
            {
                var session = await _client.Auth.SignUp(email, password);

                if (session?.User == null)
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

                var insert = await _client.From<Models.Profile>().Insert(profile);
                Profile newUser = insert.Model.ToEntity();

                return newUser;
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

        public async Task SignOut()
        {
            try
            {
                await _client.Auth.SignOut();
            }
            catch (GotrueException e)
            {
                throw new SignOutException("Abmelden fehlgeschlagen: " + e.Message);
            }
            catch (Exception e)
            {
                throw new SignOutException("Unerwarteter Logout Fehler: " + e.Message);
            }
        }

        public async Task DeleteAccount()
        {
            Guid currentUserId = Guid.Parse(_client.Auth.CurrentUser.Id);

            try
            {
                await _client.From<Models.Profile>().Where(p => p.UserId == currentUserId).Delete();
            }
            catch (Exception e)
            {
                throw new SignOutException("Löschen des Nutzers fehlgeschlagen: " + e.Message);
            }
        }

        public bool IsSignedIn()
        {
            return _client.Auth.CurrentUser != null;
        }
    }
}

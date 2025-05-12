using Supabase.Gotrue;
using System.Threading.Tasks;
using Entities;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<Session> SignIn(string email,  string password);
        Task<Profile> SignUp(string email, string password, string name, string surname);
        Task SignOut();
        bool IsSignedIn();
    }
}
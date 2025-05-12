using Repositories;
using System.Threading.Tasks;
using Entities;

namespace UseCases
{
    public class SignUpUseCase
    {
        private IUserRepository _repo;

        public SignUpUseCase(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<Profile> Execute(string email, string password, string name, string surname)
        {
            return await _repo.SignUp(email, password, name, surname);
        }
    }
}

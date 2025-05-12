using Repositories;
using Supabase.Gotrue;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UseCases
{
    public class SignInUseCase
    {
        private IUserRepository _repo;

        public SignInUseCase(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<Session> Execute(string email, string password)
        {
            return await _repo.SignIn(email, password);
        }
    }
}

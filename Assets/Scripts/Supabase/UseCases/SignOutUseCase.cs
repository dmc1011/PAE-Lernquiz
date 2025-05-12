using Repositories;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UseCases
{
    public class SignOutUseCase
    {
        private IUserRepository _repo;

        public SignOutUseCase(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task Execute()
        {
            await _repo.SignOut();
        }
    }
}

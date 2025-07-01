using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Repositories;

namespace UseCases
{
    public class CheckSessionUseCase
    {
        private IUserRepository _repo;

        public CheckSessionUseCase(IUserRepository repo)
        {
            _repo = repo;
        }

        public bool CheckLoginStatus()
        {
            return _repo.IsSignedIn();
        }
    }
}

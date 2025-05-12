using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;
using UseCases;


namespace Controllers
{
    public class UserLoginController
    {
        private SignInUseCase _signInUseCase;
        private SignUpUseCase _signUpUseCase;
        private SignOutUseCase _signOutUseCase;
        private CheckSessionUseCase _checkSessionUseCase;

        public UserLoginController(SignInUseCase signInUseCase, SignUpUseCase signUpUseCase, SignOutUseCase signOutUseCase, CheckSessionUseCase checkSessionUseCase)
        {
            _signInUseCase = signInUseCase;
            _signUpUseCase = signUpUseCase;
            _signOutUseCase = signOutUseCase;
            _checkSessionUseCase = checkSessionUseCase;
        }

        public async Task SignIn(string email, string password)
        {
            await _signInUseCase.Execute(email, password);
        }

        public bool IsSignedIn()
        {
            return _checkSessionUseCase.CheckLoginStatus();
        }

        public async Task SignUp(string email, string password, string name, string surname)
        {
            await _signUpUseCase.Execute(email, password, name, surname);
        }

        public async Task SignOut()
        {
            await _signOutUseCase.Execute();
        }
    }
}

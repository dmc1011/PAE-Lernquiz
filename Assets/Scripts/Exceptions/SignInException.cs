using System;

public class SignInException : Exception
{
    public SignInException(string message) : base(message) { }
}

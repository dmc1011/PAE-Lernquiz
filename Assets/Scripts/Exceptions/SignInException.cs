using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInException : Exception
{
    public SignInException(string message) : base(message) { }
}

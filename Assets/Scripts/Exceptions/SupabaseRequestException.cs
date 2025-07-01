using System;

public class SupabaseRequestException : Exception
{
    public SupabaseRequestException(string message) : base(message) { }
}

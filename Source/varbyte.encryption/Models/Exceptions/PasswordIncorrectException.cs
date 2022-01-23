using System;

namespace varbyte.encryption.Models.Exceptions;


public class PasswordIncorrectException : Exception
{
    public PasswordIncorrectException(string message)
        : base(message)
    {
    }
    public PasswordIncorrectException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public PasswordIncorrectException() : base()
    {
    }
}
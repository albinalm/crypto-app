using System;

namespace varbyte.encryption.Models.Exceptions;


public class ValidationFailedException : Exception
{
    public ValidationFailedException(string message)
        : base(message)
    {
    }
    public ValidationFailedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
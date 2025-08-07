using System;

namespace cmedcc_idass.backend.Exceptions;

public class UserExistsException : Exception
{
    public UserExistsException(string message) : base(message)
    {
    }
}
using System;

namespace Infrastructure.Contracts.Documents.Exceptions;

public class RegistrationNumberIsNotUniqueException : Exception
{
    public string Number { get;  }
    public RegistrationNumberIsNotUniqueException(string number)
        : base($"Document number {number} is already registered")
    {
        Number = number;
    }
}

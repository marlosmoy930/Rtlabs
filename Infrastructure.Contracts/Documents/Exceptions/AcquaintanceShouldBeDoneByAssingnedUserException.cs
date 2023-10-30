using System;

namespace Infrastructure.Contracts.Documents.Exceptions;

public class AcquaintanceShouldBeDoneByAssingnedUserException : Exception
{
    public AcquaintanceShouldBeDoneByAssingnedUserException()
        : base($"Document acquaintance should be done by assigned user")
    {
    }
}
using System;

namespace Infrastructure.Contracts.Documents.Exceptions;

public class AcquaintanceAlreadyDoneException : Exception
{
    public AcquaintanceAlreadyDoneException()
        : base($"Document acquaintance is already done")
    {
    }
}
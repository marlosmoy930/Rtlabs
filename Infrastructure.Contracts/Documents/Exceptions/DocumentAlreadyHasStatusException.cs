using System;
using ESD.Domain.Documents.Entities;

namespace Infrastructure.Contracts.Documents.Exceptions;

public class DocumentAlreadyHasStatusException : Exception
{
    public Document Document { get; }

    public DocumentAlreadyHasStatusException(Document document)
        : base($"Document is already {document.Status}")
    {
        Document = document;
    }
}

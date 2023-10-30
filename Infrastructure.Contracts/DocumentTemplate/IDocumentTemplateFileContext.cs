using ESD.Domain.DocumentTemplate.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Contracts.DocumentTemplate;
public interface IDocumentTemplateFileContext
{
    Task<Guid> SaveFileAsync(DocumentTemplateFile documentFile, CancellationToken cancellationToken);
    Task<DocumentTemplateFile> GetFileAsync(Guid fileId, CancellationToken cancellationToken);
    DocumentTemplateFile GetFile(Guid fileId);
}
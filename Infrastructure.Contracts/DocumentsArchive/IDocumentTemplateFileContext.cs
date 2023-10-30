using System;
using System.Threading;
using System.Threading.Tasks;
using ESD.Domain.DocumentsArchive.Entities;

namespace Infrastructure.Contracts.DocumentsArchive;

public interface IDocumentsArchiveFileContext
{
    Task<Guid> SaveFileAsync(DocumentsArchiveFile documentFile, CancellationToken cancellationToken);
    Task<DocumentsArchiveFile> GetFileAsync(Guid fileId, CancellationToken cancellationToken); 
}
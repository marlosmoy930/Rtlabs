using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ESD.Domain.Documents.Entities;

namespace Infrastructure.Contracts.Documents
{
    public interface IDigitalSignatureFileContext
    {
        Task<Guid> SaveDigitalSignatureFileAsync(DigitalSignatureFile digitalSignatureFile, CancellationToken cancellationToken);
        
        Task<DigitalSignatureFile> GetDigitalSignatureFileAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<DigitalSignatureFile>> GetDigitalSignatureFilesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}

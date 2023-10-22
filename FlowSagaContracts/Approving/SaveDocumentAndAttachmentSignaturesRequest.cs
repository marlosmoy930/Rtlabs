using ESD.Domain.Documents.Entities;
using ESD.Domain.Documents.Enums;

namespace FlowSagaContracts.Approving
{
    public class SaveDocumentAndAttachmentSignaturesRequest
    {
        public SignedDocumentWithAttachments SignedDocumentDataAndUserAttachments { get; set; }

        public Guid SignerId { get; set; }

        public string SignerLogin { get; set; }

        public DateTime CreatedUtc { get; set; }

        public SignedUserType Format { get; set; }
    }
}

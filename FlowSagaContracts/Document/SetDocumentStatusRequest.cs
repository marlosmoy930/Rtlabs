using ESD.Domain.Documents.Enums;

namespace FlowSagaContracts.Document
{
    public class SetDocumentStatusRequest
    {
        public Guid SagaId { get; set; }

        public int DocumentId { get; set; }

        public DocumentStatus DocumentStatus { get; set; }
    }
}

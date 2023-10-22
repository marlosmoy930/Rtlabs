namespace FlowSagaContracts.Document;

public class ChangeDocumentRegistrationRequest
{
    public Guid DocumentRegistrationUserInteractionSagaId { get; set; }
    public int DocumentId { get; set; }
}
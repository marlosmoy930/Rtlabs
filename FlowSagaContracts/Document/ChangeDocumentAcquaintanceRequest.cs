namespace FlowSagaContracts.Document;

public class ChangeDocumentAcquaintanceRequest
{
    public Guid DocumentAcquaintanceUserInteractionSagaId { get; set; }
    public int DocumentId { get; set; }
    public int StageId { get; set; }
}
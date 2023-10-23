namespace FlowSagaContracts.SuccessionPool;

public class UpdateSuccessionPoolInclusionRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }
}
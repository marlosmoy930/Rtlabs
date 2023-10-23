namespace FlowSagaContracts.SuccessionPool;

public class UpdateSuccessionPoolOrderRegistrationDataRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }
}
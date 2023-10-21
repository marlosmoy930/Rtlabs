namespace FlowSagaContracts.ApproveStaffingTableStatement;

public class RegistrationTaskForSagaRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }
}

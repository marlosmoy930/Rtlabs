using ESD.Domain.StaffProcesses.Enums;

public class UpdateSuccessionPoolOrderStateRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}
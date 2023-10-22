using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.ContestResults;

public class UpdateContestResultStateRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}
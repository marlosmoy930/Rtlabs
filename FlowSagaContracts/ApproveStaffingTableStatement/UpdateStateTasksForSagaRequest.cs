using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.ApproveStaffingTableStatement;

public class UpdateStateTasksForSagaRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }        
}

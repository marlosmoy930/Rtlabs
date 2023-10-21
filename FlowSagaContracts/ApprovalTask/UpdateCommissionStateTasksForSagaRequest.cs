using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.ApprovalTask;

public class UpdateCommissionStateTasksForSagaRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }
    
    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
    
    public Guid DocumentGuid { get; set; }

    public int DocumentVersion { get; set; }
}
using ESD.Domain.StaffProcesses.Enums;
using FlowSagaContracts.Approving;

namespace FlowSagaContracts.ApprovalTask;

public class ApprovalTaskResolvedData
{
    public int ApprovalTaskId { get; init; }

    public ApprovalActionType ActionType { get; init; }

    public ApprovalType ResolvedStepApprovalType { get; init; }

    public Guid UserId { get; init; }

    public string UserName { get; set; }

    public DateTime DateTimeUtc { get; init; }
    
    public int ResolvedStepIndex { get; init; }
    
    public int TotalSteps { get; init; }
    
    public Guid? SagaOwnerId { get; set; }
    
    public ApprovalSagaStepResultType? ResolvedStepResult { get; set; }
    
    public string Comment { get; set; }

    public bool IsGovernmentKeyApprovalProcessExternalTask { get; set; }
}

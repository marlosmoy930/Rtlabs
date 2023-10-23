using ESD.Domain.StaffProcesses.Enums;
using FlowSagaContracts.Approving;

namespace FlowSagaContracts.StaffProcessEvent;

public class ApprovalTaskResolvedEventRequest
{
    public int ApprovalTaskId { get; init; }

    public ApprovalActionType ActionType { get; init; }

    public ApprovalType ApprovalTaskType { get; init; }

    public int DocumentId { get; init; }

    public Guid UserId { get; init; }

    public string UserName { get; init; }

    public DateTime DateTimeUtc { get; init; }
}

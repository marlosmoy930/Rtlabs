using ESD.Domain.StaffProcesses.Entities;
using FlowSagaContracts.Approving.ApprovalChain;

namespace FlowSagaContracts.ApprovalChainModification;

public class NotifyApprovalChainModificationDataEventRequest
{
    public List<ApprovalSagaStep> BeforeUpdateSagaSteps { get; set; }

    public List<ApprovalSagaStep> AfterUpdateSagaSteps { get; set; }

    public int CurrentStepIndex { get; set; }

    public int DocumentId { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public List<ApprovalStep> Steps { get; set; }
}

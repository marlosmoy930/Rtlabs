using FlowSagaContracts.Approving.ApprovalChain;

using MassTransit;

namespace FlowSagaContracts.ApprovalChainModification;

public class ApprovalChainModificationData : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }

    public List<ApprovalSagaStep> Steps { get; set; }

    public DateTime PerformedUtc { get; set; } 
}

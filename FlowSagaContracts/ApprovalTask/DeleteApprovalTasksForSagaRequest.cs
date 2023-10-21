using FlowSagaContracts.Approving.ApprovalChain;

namespace FlowSagaContracts.ApprovalTask
{
    public class DeleteApprovalTasksForSagaRequest
    {
        public Guid ApprovalSagaCorrelationId { get; set; }

        public DateTime PerformedUtc { get; set; }

        public List<ApprovalSagaStep> ApprovalSteps { get; set; }

        public int? ExcludedTaskId { get; set; }
    }
}

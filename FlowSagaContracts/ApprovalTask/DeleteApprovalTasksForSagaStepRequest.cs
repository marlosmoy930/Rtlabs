namespace FlowSagaContracts.ApprovalTask
{
    public class DeleteApprovalTasksForSagaStepRequest
    {
        public Guid ApprovalSagaCorrelationId { get; set; }

        public DateTime PerformedUtc { get; set; }

        public int ApprovalStepIndex { get; set; }

        public int? ExcludedTaskId { get; set; }
    }
}

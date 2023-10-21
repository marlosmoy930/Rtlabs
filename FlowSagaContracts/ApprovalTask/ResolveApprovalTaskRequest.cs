using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.ApprovalTask
{
    public class ResolveApprovalTaskRequest
    {
        public int ApprovalTaskId { get; set; }

        public ApprovalActionType ApprovalAction { get; set; }

        public DateTime ResolvedUtc { get; set; }
    }
}

using Common.Exceptions;
using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.Approving.ApprovalChain;

public class ApprovalSagaStep
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Index { get; set; }

    public ApprovalType ApprovalType { get; set; }

    public IList<ApprovalTaskAssigneeWithTemplate> ApprovalTaskAssignees { get; set; }

    public int? DaysToApprove { get; set; }

    public bool CanBeRejected { get; set; }

    public bool CanBeSentToReview { get; set; }

    public bool CanAttachDocuments { get; set; }
    
    public bool IsReturnedOnCheckFail { get; set; }

    public int? CustomActionTemplateAtStartId { get; set; }

    public int? CustomActionTemplateBeforeEndId { get; set; }

    public IList<ApprovedAssignee> ApprovedAssignees { get; set; } = new List<ApprovedAssignee>();

    public string Message { get; set; }

    public ApprovalSagaStepResultType? Result { get; set; }

    public void SetResult(ApprovalActionData approvalData)
    {
        Result = ApplyApprovalAction(approvalData);
    }

    private ApprovalSagaStepResultType? ApplyApprovalAction(ApprovalActionData approvalData)
    {
        if (ApprovedAssignees.Any(approvedAssignee => approvedAssignee.ApprovedAssigneeId == approvalData.AssigneeId
                                                        && approvedAssignee.ApprovalTaskId == approvalData.ApprovalTaskId))
        {
            return null;
        }

        if (approvalData.ActionType == ApprovalActionType.Cancel)
        {
            return ApprovalSagaStepResultType.Canceled;
        }

        if (approvalData.ActionType == ApprovalActionType.Reject)
        {
            if (!CanBeRejected)
            {
                throw new BusinessException("Задачи текущего шага не могут быть отклонены.");
            }

            return ApprovalSagaStepResultType.Rejected;
        }

        if (approvalData.ActionType == ApprovalActionType.Review)
        {
            if (!CanBeSentToReview)
            {
                throw new BusinessException("Задачи текущего шага не могут быть отправлены на доработку.");
            }

            return ApprovalSagaStepResultType.Review;
        }

        ApprovedAssignees.Add(new ApprovedAssignee(approvalData.AssigneeId, approvalData.ApprovalTaskId, DateTime.UtcNow));

        if (ApprovedAssignees.Count == ApprovalTaskAssignees.Count)
        {
            return ApprovalSagaStepResultType.Approved;
        }

        return ApprovalSagaStepResultType.NeedsMoreApprovals;
    }

    public class ApprovedAssignee
    {
        public Guid ApprovedAssigneeId { get; private set; }

        public int ApprovalTaskId { get; private set; }

        public DateTime ApprovedAtUtc { get; private set; }

        public ApprovedAssignee(Guid approvedAssigneeId, int approvalTaskId, DateTime approvedAtUtc)
        {
            ApprovedAssigneeId = approvedAssigneeId;
            ApprovalTaskId = approvalTaskId;
            ApprovedAtUtc = approvedAtUtc;
        }
    }
}

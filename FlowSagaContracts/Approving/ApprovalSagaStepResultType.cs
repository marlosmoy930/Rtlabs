namespace FlowSagaContracts.Approving;

public enum ApprovalSagaStepResultType
{
    Approved = 1,
    Rejected,
    Awaiting,
    NeedsMoreApprovals,
    Review,
    Canceled
}

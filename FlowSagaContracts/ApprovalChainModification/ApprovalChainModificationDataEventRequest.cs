using FlowSagaContracts.Approving.ApprovalChain;

namespace FlowSagaContracts.ApprovalChainModification;

public class ApprovalChainModificationDataEventRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public ApprovalSagaStep CurrentApprovalStep { get; set; }

    public int DocumentId { get; set; }
    
    public int StaffProcessStageId { get; set; }

    public Dictionary<string, string> TaskNameTemplateSubstitutions { get; set; }

    public DateTime PerformedUtc { get; set; }

    public ApprovalSagaStep UpdatedApprovalStep { get; set; }  
}

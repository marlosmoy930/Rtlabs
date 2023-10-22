using ESD.Domain.Documents.Entities;
using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;

using MassTransit;

namespace FlowSagaContracts.Approving;

public class ApprovalActionData: CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }

    public int ApprovalTaskId { get; set; }

    public ApprovalActionType ActionType { get; set; }

    public Guid AssigneeId { get; set; }

    public string AssigneeLogin { get; set; }

    public int DocumentId { get; set; }

    public DateTime PerformedUtc { get; set; }

    public SignedDocumentWithAttachments? SignedDocumentWithAttachments { get; set; }

    public DocumentAttachmentSignature? DocumentAttachmentSignature { get; set; }

    public string Comment { get; set; }

    public bool IsGovernmentKeyApprovalProcessExternalAction { get; set; }
    
    public SignedUserType? StampFormat { get; set; }
}

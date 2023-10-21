using ESD.Domain.Enums;
using FlowSagaContracts.Approving.ApprovalChain;

namespace FlowSagaContracts.ApprovalTask;

public class CreateApprovalTasksRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public ApprovalSagaStep ApprovalStep { get; set; }

    public Guid InitiatorId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid? SubjectId { get; set; }
    
    public Guid? CommissionMeetingId { get; set; }

    public int DocumentId { get; set; }

    public int StaffProcessStageId { get; set; }

    public Dictionary<string,string> TaskNameTemplateSubstitutions { get; set; }

    public Dictionary<FlowSagaAdditionalDataKey, string> AdditionalData { get; set; }
}

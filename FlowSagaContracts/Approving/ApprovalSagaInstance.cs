using FlowSagaContracts.Approving.ApprovalChain;

using MassTransit;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowSagaContracts.Approving;

public class ApprovalSagaInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string? CurrentState { get; set; }

    public string? LogCorrelationId { get; set; }

    public DateTime StartedAt { get; set; }

    public Guid? UserId { get; set; }

    public string? UserEmail { get; set; }

    public Guid ParentSagaCorrelationId { get; set; }

    public int ParentSagaFlowTaskId { get; set; }

    public string? ParentSagaAddress { get; set; }

    public int CurrentStepIndex { get; set; }

    private bool approvalDataExtracted = false;

    private ApprovalData approvalData;

    [NotMapped]
    public ApprovalData ApprovalData
    {
        get
        {
            if (!approvalDataExtracted)
            {
                approvalData = JsonConvert.DeserializeObject<ApprovalData>(ApprovalDataJson!);
                approvalDataExtracted = true;
            }
            return approvalData;
        }
        set
        {
            ApprovalDataJson = JsonConvert.SerializeObject(value);
        }
    }
    
    public List<ApprovalSagaStep> GetSteps() => ApprovalData.ApprovalChain.Steps;

    public void SetSteps(List<ApprovalSagaStep> value)
    {
        ApprovalData.ApprovalChain = new ApprovalSagaChain { Steps = value };
        ApprovalDataJson = JsonConvert.SerializeObject(approvalData);
    }

    public int DocumentId { get; set; }

    public int StaffProcessStageId { get; set; }

    private string? approvalDataJson;

    public string? ApprovalDataJson
    {
        get
        {
            return approvalDataJson;
        }

        set
        {
            approvalDataJson = value;
            approvalDataExtracted = false;
        }
    }

    public Dictionary<string, string> GetTaskNameTemplateSubstitutions() => ApprovalData.TaskNameTemplateSubstitutions;

    public Guid GetDepartmentId() => ApprovalData.DepartmentId;

    public Guid? GetSubjectId() => ApprovalData.SubjectId;

    public Guid? GetCommissionMeetingId() => ApprovalData.CommissionMeetingId;
}

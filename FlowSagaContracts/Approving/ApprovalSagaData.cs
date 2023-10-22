using ESD.Domain.Enums;

using FlowSagaContracts.Approving.ApprovalChain;

using Newtonsoft.Json;

namespace FlowSagaContracts.Approving;

public class ApprovalSagaData
{
    //public string ApprovalStepsJson { get; set; }
    
    public string ApprovalDataJson { get; set; }

    public ApprovalSagaStepResultType? ApprovalSagaResultType { get; set; }

    public int DocumentId { get; set; }

    public static ApprovalSagaData Create(
        List<ApprovalSagaStep> stepConfigs, 
        int documentId, 
        int staffProcessStageId,
        Guid departmentId,
        Dictionary<string, string> taskNameTemplateSubstitutions,
        Dictionary<FlowSagaAdditionalDataKey, string> additionalData,
        StaffProcessStageCode stageCode)
    {
        var approvalData = new ApprovalData
        {
            ApprovalChain = new ApprovalSagaChain { Steps = stepConfigs },
            TaskNameTemplateSubstitutions = taskNameTemplateSubstitutions,
            DepartmentId = departmentId,
            AdditionalData = additionalData,
            StageCode = stageCode,
        };
        
        var approvalDataJson = JsonConvert.SerializeObject(approvalData);
        
        return new ApprovalSagaData
        { 
            DocumentId = documentId,
            StaffProcessStageId = staffProcessStageId,
            ApprovalDataJson = approvalDataJson
        };
    }

    public int StaffProcessStageId { get; set; }

    //public string? TaskNameTemplateSubstitutionsJson { get; set; }
}

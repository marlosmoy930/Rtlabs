using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services;

using Services.Models.ApprovalChain;

using FlowSagaContracts.Approving;
using ESD.Domain.Enums;

namespace FlowSagas.CommissionProcess;

public  class CommissionFlowSagaData : IFlowSagaWithUserInteractionSagaData, ILaunchSettings 
{
    public ApprovalSagaStepResultType? ApprovalSagaResultType { get; set; }    

    public ApprovalChain ApprovalChain { get; set; }

    public Guid InitiatorId { get; set; }

    public Guid DepartmentId { get; set; }

    public int DocumentId { get; set; }

    public Guid DocumentGuid { get; set; }

    public int DocumentVersion { get; set; }

    public Dictionary<string, string> TaskNameTemplateSubstitutions { get; set; }
    
    public string UserInteractionSagaResult { get; set; }

    public Dictionary<FlowSagaAdditionalDataKey, string> AdditionalData { get; set; }
    
    public StaffProcessStageCode StageCode { get; set; }
}

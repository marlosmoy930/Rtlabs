using ESD.Domain.Enums;

using FlowSagaContracts.Approving;

using Services.Models.ApprovalChain;

namespace FlowSagas;

public interface IFlowSagaDataBase
{
    ApprovalChain ApprovalChain { get; set; }

    ApprovalSagaStepResultType? ApprovalSagaResultType { get; set; }

    Dictionary<string, string> TaskNameTemplateSubstitutions { get; set; }

    int DocumentId { get; set; }

    Guid DepartmentId { get; set; }

    Dictionary<FlowSagaAdditionalDataKey, string> AdditionalData { get; set; }
    
    StaffProcessStageCode StageCode { get; set; }
}

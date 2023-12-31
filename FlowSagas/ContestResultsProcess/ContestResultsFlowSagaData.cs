﻿using ESD.Domain.Enums;
using FlowSagaContracts.Approving;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services;
using Services.Models.ApprovalChain;

namespace FlowSagas.ContestResultsProcess;

public class ContestResultsFlowSagaData : IFlowSagaWithUserInteractionSagaData, ILaunchSettings
{
    public ApprovalSagaStepResultType? ApprovalSagaResultType { get; set; }

    public ApprovalChain ApprovalChain { get; set; }

    public Guid InitiatorId { get; set; }

    public Guid DepartmentId { get; set; }

    public int DocumentId { get; set; }

    public Dictionary<string, string> TaskNameTemplateSubstitutions { get; set; }

    public string UserInteractionSagaResult { get; set; }

    public StaffProcessStageCode StageCode { get; set; }

    public Dictionary<FlowSagaAdditionalDataKey, string> AdditionalData { get; set; }

}

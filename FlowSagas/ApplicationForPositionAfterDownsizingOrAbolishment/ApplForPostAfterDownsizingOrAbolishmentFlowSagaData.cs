using ESD.Domain.Enums;

using FlowSagaContracts.Approving;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services;

using Services.Models.ApprovalChain;

namespace FlowSagas.ApplicationForPositionAfterDownsizingOrAbolishment;

/// <summary>
/// Должно быть название ApplicationForPositionAfterDownsizingOrAbolishmentSagaData.
/// Но в этом случае длина названия таблицы для Instances превысит 63 символа и будет обрезана Postgres без сообщения об ошибки миграции.
/// Приходится сокращать название "изнутри".
/// </summary>
public class ApplForPostAfterDownsizingOrAbolishmentFlowSagaData : ILaunchSettings, IFlowSagaDataBase
{
    public ApprovalSagaStepResultType? ApprovalSagaResultType { get; set; }

    public ApprovalChain ApprovalChain { get; set; }

    public int DocumentId { get; set; }

    public Guid DepartmentId { get; set; }

    public Dictionary<string, string> TaskNameTemplateSubstitutions { get; set; }

    public Dictionary<FlowSagaAdditionalDataKey, string> AdditionalData { get; set; }
    
    public StaffProcessStageCode StageCode { get; set; }
}

using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Notification;
using FlowSagaContracts.StaffVacancyCompetition;
using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.StaffVacancyCompetition;

public class  StaffVacancyCompetitionFlowSaga : 
    FlowSaga<StaffVacancyCompetitionFlowSagaData, 
    IStaffVacancyCompetitionFlowContext>
{
    private readonly ILogger<StaffVacancyCompetitionFlowSaga> _logger;

    public StaffVacancyCompetitionFlowSaga(IServiceProvider serviceProvider,
        ILogger<StaffVacancyCompetitionFlowSaga> logger) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        UpdateState(StaffProcessStageState.InProgress)
            .Then(Create.StartApprovalSagaWithNotification())
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected,
                UpdateState(StaffProcessStageState.Rejected))
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review,
                UpdateState(StaffProcessStageState.Review))
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                Do(Create.StartDocumentRegistrationUserInteractionSaga()
                    .Then(Create.SendDocumentRegisteredNotificationRequest())
                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                    .Then(UpdateState(StaffProcessStageState.Completed))
                    )
                );
    }

    private FlowTaskChain<StaffVacancyCompetitionFlowSagaData, IStaffVacancyCompetitionFlowContext> UpdateState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateStaffVacancyStateRequest>();
        monolog.SetRequestFactory(s =>
            new UpdateStaffVacancyStateRequest
            {
                State = state,
                DocumentId = s.Data.DocumentId
            });

        return monolog.Send();
    }
}

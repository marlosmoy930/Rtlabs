using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Notification;
using FlowSagaContracts.ReserveVacancyCompetition;
using FlowSagaContracts.UserInteraction;
using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;


namespace FlowSagas.ReserveVacancyCompetition;

public class
    ReserveVacancyCompetitionFlowSaga : FlowSaga<ReserveVacancyCompetitionFlowSagaData,
        IReserveVacancyCompetitionFlowContext>
{
    private readonly ILogger<ReserveVacancyCompetitionFlowSaga> _logger;

    public ReserveVacancyCompetitionFlowSaga(IServiceProvider serviceProvider,
        ILogger<ReserveVacancyCompetitionFlowSaga> logger) : base(serviceProvider)
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

    private FlowTaskChain<ReserveVacancyCompetitionFlowSagaData, IReserveVacancyCompetitionFlowContext> UpdateState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateReserveVacancyStateRequest>();
        monolog.SetRequestFactory(s =>
            new UpdateReserveVacancyStateRequest
            {
                State = state,
                DocumentId = s.Data.DocumentId,
            });

        return monolog.Send();
    }

    private FlowTaskChain<ReserveVacancyCompetitionFlowSagaData, IReserveVacancyCompetitionFlowContext>
        StartUserInteractionSaga()
    {
        return Create
            .ChildSaga<UserInteractionSagaData, IApprovalSagaContext>("User Interaction Saga")
            .SetRequestFactory(s => new UserInteractionSagaData()
            {
                DocumentId = s.CurrentSagaSpace.Data.DocumentId,
            })
            .SetChunkResponseProcessor((s, chunkIndex) =>
            {
                _logger.LogInformation("User interaction result: {0}", s.ChildSagaSpace.Data.EndData);
            })
            .Start();
        ;
    }
}

using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;
using FlowSagaContracts.Approving;
using FlowSagaContracts.Notification;
using FlowSagaContracts.UserInteraction;
using FlowSagaContracts.VacancyCandidateRejection;
using FlowSagas.FlowChainBuilding;
using Microsoft.Extensions.Logging;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.VacancyCandidateRejection;

public class VacancyCandidateRejectionFlowSaga : FlowSaga<VacancyCandidateRejectionFlowSagaData,
    IVacancyCandidateRejectionFlowContext>
{
    private readonly ILogger<VacancyCandidateRejectionFlowSaga> _logger;

    public VacancyCandidateRejectionFlowSaga(IServiceProvider serviceProvider,
        ILogger<VacancyCandidateRejectionFlowSaga> logger) : base(serviceProvider)
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
                Do(Create.SendDocumentApprovedNotificationRequest())
                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                    .Then(UpdateState(StaffProcessStageState.Completed))
                );
    }

    private FlowTaskChain<VacancyCandidateRejectionFlowSagaData, IVacancyCandidateRejectionFlowContext> UpdateState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateCandidateRejectionStateRequest>();
        monolog.SetRequestFactory(s =>
            new UpdateCandidateRejectionStateRequest
            {
                State = state,
                DocumentId = s.Data.DocumentId,
            });

        return monolog.Send();
    }
}

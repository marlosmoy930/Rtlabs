using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;
using FlowSagaContracts.Approving;
using FlowSagaContracts.Notification;
using FlowSagaContracts.UserInteraction;
using FlowSagaContracts.VacancyCandidateInvitation;
using FlowSagas.FlowChainBuilding;
using Microsoft.Extensions.Logging;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.VacancyCandidateInvitation;

public class VacancyCandidateInvitationFlowSaga : FlowSaga<VacancyCandidateInvitationFlowSagaData,
    IVacancyCandidateInvitationFlowContext>
{
    private readonly ILogger<VacancyCandidateInvitationFlowSaga> _logger;

    public VacancyCandidateInvitationFlowSaga(IServiceProvider serviceProvider,
        ILogger<VacancyCandidateInvitationFlowSaga> logger) : base(serviceProvider)
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

    private FlowTaskChain<VacancyCandidateInvitationFlowSagaData, IVacancyCandidateInvitationFlowContext> UpdateState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateCandidateInvitationStateRequest>();
        monolog.SetRequestFactory(s =>
            new UpdateCandidateInvitationStateRequest
            {
                State = state,
                DocumentId = s.Data.DocumentId,
            });

        return monolog.Send();
    }
}

using ESD.Domain.StaffProcesses.Enums;
using ESD.Domain.Documents.Enums;
using FlowSagaContracts.ApprovalTask;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PersonalReferenceForSalary;

using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;
using FlowSagaContracts.Notification;

namespace FlowSagas.PersonalReferenceProcess;

public class PersonalReferenceFlowSaga : FlowSaga<PersonalReferenceFlowSagaData, IPersonalReferenceForSalaryFlowSagaContext>
{
    public PersonalReferenceFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void ConfigureFlow()
    {
        UpdateState(StaffProcessStageState.InProgress)
            .Then(Create.StartApprovalSaga())
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected,
                UpdateState(StaffProcessStageState.Rejected)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review,
                UpdateState(StaffProcessStageState.Review)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected
                && s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Review,
                Do(
                        UpdateState(StaffProcessStageState.Completed)
                    )                    
                    .Then(Create.SendDocumentApprovedNotificationRequest())
                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
            );
    }

    private FlowTaskChain<PersonalReferenceFlowSagaData, IPersonalReferenceForSalaryFlowSagaContext> UpdateState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdatePersonalReferenceStateForSagaRequest>();

        monolog.SetRequestFactory(s =>
        {
            return new UpdatePersonalReferenceStateForSagaRequest()
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                State = state,
                DocumentId = s.Data.DocumentId,
                DocumentVersion = s.Data.DocumentVersion,
                DocumentGuid = s.Data.DocumentGuid
            };
        });

        return monolog.Send();
    }
}

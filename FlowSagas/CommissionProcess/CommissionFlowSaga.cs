using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.ApprovalTask;
using FlowSagaContracts.Approving;
using FlowSagaContracts.Commission;
using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.CommissionProcess;

public class CommissionFlowSaga : FlowSaga<CommissionFlowSagaData, ICommissionFlowSagaContext>
{
    public CommissionFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void ConfigureFlow()
    {
        UpdateCommissionState(StaffProcessStageState.InProgress)
            .Then(Create.StartApprovalSagaWithNotification())
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected,
                UpdateCommissionState(StaffProcessStageState.Rejected)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review,
                UpdateCommissionState(StaffProcessStageState.Review)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected
                    && s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Review,
                Do(
                    UpdateCommissionState(StaffProcessStageState.Completed)
                    )
                    .Then(Create.StartDocumentRegistrationUserInteractionSaga()
                        .ThenIf(
                            s => s.Data.UserInteractionSagaResult == "Registered",
                            UpdateCommissionWithRegistrationData()
                        )
                        .Then(Create.SendDocumentRegisteredNotificationRequest())
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                    )
            );
    }

    private FlowTaskChain<CommissionFlowSagaData, ICommissionFlowSagaContext> UpdateCommissionWithRegistrationData()
    {
        return Create.Monolog<UpdateCommissionWithRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdateCommissionWithRegistrationDataRequest
            {
                DocumentId = s.Data.DocumentId
            })
            .Send();
    }

    private FlowTaskChain<CommissionFlowSagaData, ICommissionFlowSagaContext> UpdateCommissionState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateCommissionStateTasksForSagaRequest>();

        monolog.SetRequestFactory(s => new UpdateCommissionStateTasksForSagaRequest
        {
            ApprovalSagaCorrelationId = s.CorrelationId,
            State = state,
            DocumentId = s.Data.DocumentId,
            DocumentVersion = s.Data.DocumentVersion,
            DocumentGuid = s.Data.DocumentGuid
        });

        return monolog.Send();
    }
}

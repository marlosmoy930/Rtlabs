using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;

using FlowSagaContracts.AppointmentWorkingWithStateSecret;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagas.AppointmentJuniorPositionGroup;
using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.AppointmentWorkingWithStateSecret;

public class AppointmentWorkingWithStateSecretFlowSaga
    : FlowSaga<AppointmentWorkingWithStateSecretFlowSagaData, IAppointmentWorkingWithStateSecretFlowContext>
{
    public AppointmentWorkingWithStateSecretFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider) {}

    protected override void ConfigureFlow()
    {
        DoIf(
            IsStatementStage,
            Create.StartApprovalSagaWithNotification()
                .ThenIf(
                    s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                    Do(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                )
        );

        DoIf(
            IsOrderStage,
             Create.StartApprovalSagaWithNotification()
                .ThenIf(
                    s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                    Do(Create.SendSetDocumentStatusRequest(DocumentStatus.OnRegistration)
                        .Then(Create.StartDocumentRegistrationUserInteractionSaga())
                        .ThenIf(WasRegistered, UpdateOrderRegistrationData())
                        .Then(Create.SendDocumentRegisteredNotificationRequest())
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished)
                            .Then(Create.StartDocumentAcquaintanceUserInteractionSaga())))
                )
        );
    }
    private bool WasAcquainted(FlowSpace<AppointmentWorkingWithStateSecretFlowSagaData, IAppointmentWorkingWithStateSecretFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool IsStatementStage(FlowSpace<AppointmentWorkingWithStateSecretFlowSagaData, IAppointmentWorkingWithStateSecretFlowContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.AppointmentWorkingWithStateSecretStatement or
            StaffProcessStageCode.AppointmentWorkingWithStateSecretReplacementStatement or
            StaffProcessStageCode.AppointmentWorkingWithStateSecretReplacementStatementWithPresentation;
    }

    private bool IsOrderStage(FlowSpace<AppointmentWorkingWithStateSecretFlowSagaData, IAppointmentWorkingWithStateSecretFlowContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.AppointmentWorkingWithStateSecretOrder or
            StaffProcessStageCode.AppointmentWorkingWithStateSecretReplacementOrder;
    }

    private bool WasRegistered(FlowSpace<AppointmentWorkingWithStateSecretFlowSagaData, IAppointmentWorkingWithStateSecretFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<AppointmentWorkingWithStateSecretFlowSagaData, IAppointmentWorkingWithStateSecretFlowContext> UpdateOrderRegistrationData()
    {
        return Create.Monolog<UpdatePositionAppointmentOrderRegistrationDataRequest>()
            .SetRequestFactory(s => 
                new UpdatePositionAppointmentOrderRegistrationDataRequest
                {
                    DocumentId = s.Data.DocumentId,
                })
            .Send();
    }
}

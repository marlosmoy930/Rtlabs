using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;

using FlowSagaContracts.AppointmentJuniorPositionGroup;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagas.AppointmentFromReserveOrder;
using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.AppointmentJuniorPositionGroup;

public class AppointmentJuniorPositionGroupFlowSaga
    : FlowSaga<AppointmentJuniorPositionGroupFlowSagaData, IAppointmentJuniorPositionGroupFlowContext>
{
    public AppointmentJuniorPositionGroupFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

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
                        .Then(Create.SendDocumentRegisteredNotificationRequest()
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished)
                        .Then(Create.StartDocumentAcquaintanceUserInteractionSaga()))))                
                )
        );
    }

    private bool WasAcquainted(FlowSpace<AppointmentJuniorPositionGroupFlowSagaData, IAppointmentJuniorPositionGroupFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool IsStatementStage(FlowSpace<AppointmentJuniorPositionGroupFlowSagaData, IAppointmentJuniorPositionGroupFlowContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.AppointmentJuniorPositionGroupStatement or
            StaffProcessStageCode.AppointmentJuniorPositionGroupReplacementStatement or
            StaffProcessStageCode.AppointmentJuniorPositionGroupReplacementStatementWithPresentation;
    }

    private bool IsOrderStage(FlowSpace<AppointmentJuniorPositionGroupFlowSagaData, IAppointmentJuniorPositionGroupFlowContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.AppointmentJuniorPositionGroupOrder or
            StaffProcessStageCode.AppointmentJuniorPositionGroupReplacementOrder;
    }

    private bool WasRegistered(FlowSpace<AppointmentJuniorPositionGroupFlowSagaData, IAppointmentJuniorPositionGroupFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<AppointmentJuniorPositionGroupFlowSagaData, IAppointmentJuniorPositionGroupFlowContext> UpdateOrderRegistrationData()
    {
        return Create.Monolog<UpdatePositionAppointmentOrderRegistrationDataRequest>()
            .SetRequestFactory(s =>
                new UpdatePositionAppointmentOrderRegistrationDataRequest
                {
                    DocumentId = s.Data.DocumentId
                })
            .Send();
    }
}

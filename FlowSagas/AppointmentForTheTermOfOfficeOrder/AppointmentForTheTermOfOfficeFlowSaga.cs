using ESD.Domain.Documents.Enums;

using FlowSagaContracts.AppointmentForTheTermOfOfficeOrder;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagas.AppointmentForPeriodOfEmployeeAbsence;
using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.AppointmentForTheTermOfOfficeOrder;

public class AppointmentForTheTermOfOfficeOrderFlowSaga : FlowSaga<AppointmentForTheTermOfOfficeOrderFlowSagaData, IAppointmentForTheTermOfOfficeOrderFlowContext>
{
    public AppointmentForTheTermOfOfficeOrderFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void ConfigureFlow()
    {
        Create.StartApprovalSagaWithNotification()
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                Do(Create.SendSetDocumentStatusRequest(DocumentStatus.OnRegistration)
                    .Then(Create.StartDocumentRegistrationUserInteractionSaga())
                    .ThenIf(WasRegistered, UpdateAppointmentOrderRegistrationData())
                    .Then(Create.SendDocumentRegisteredNotificationRequest())
                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished)
                    .Then(Create.StartDocumentAcquaintanceUserInteractionSaga()))                     
                )
            );
    }

    private bool WasAcquainted(FlowSpace<AppointmentForTheTermOfOfficeOrderFlowSagaData, IAppointmentForTheTermOfOfficeOrderFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool WasRegistered(FlowSpace<AppointmentForTheTermOfOfficeOrderFlowSagaData, IAppointmentForTheTermOfOfficeOrderFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<AppointmentForTheTermOfOfficeOrderFlowSagaData, IAppointmentForTheTermOfOfficeOrderFlowContext> UpdateAppointmentOrderRegistrationData()
    {
        return Create.Monolog<UpdatePositionAppointmentOrderRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdatePositionAppointmentOrderRegistrationDataRequest
            {
                DocumentId = s.Data.DocumentId,
            })
            .Send();
    }
}

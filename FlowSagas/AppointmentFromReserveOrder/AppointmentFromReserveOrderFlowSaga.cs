using ESD.Domain.Documents.Enums;

using FlowSagaContracts.AppointmentFromReserveOrder;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagas.FlowChainBuilding;
using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.AppointmentFromReserveOrder;

public class AppointmentFromReserveOrderFlowSaga : FlowSaga<AppointmentFromReserveOrderFlowSagaData,
        IAppointmentFromReserveOrderFlowContext>
{
    private readonly ILogger<AppointmentFromReserveOrderFlowSaga> _logger;

    public AppointmentFromReserveOrderFlowSaga(
        IServiceProvider serviceProvider,
        ILogger<AppointmentFromReserveOrderFlowSaga> logger)
            : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        Create.StartApprovalSagaWithNotification()
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                Do(Create.SendSetDocumentStatusRequest(DocumentStatus.OnRegistration)
                    .Then(Create.StartDocumentRegistrationUserInteractionSaga())
                    .ThenIf(WasRegistered, UpdateOrderRegistrationData())
                    .Then(Create.SendDocumentRegisteredNotificationRequest())
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished)
                            .Then(Create.StartDocumentAcquaintanceUserInteractionSaga()))
                    )
            );
    }

    private bool WasAcquainted(FlowSpace<AppointmentFromReserveOrderFlowSagaData, IAppointmentFromReserveOrderFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool WasRegistered(FlowSpace<AppointmentFromReserveOrderFlowSagaData, IAppointmentFromReserveOrderFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<AppointmentFromReserveOrderFlowSagaData, IAppointmentFromReserveOrderFlowContext> UpdateOrderRegistrationData()
    {
        return Create.Monolog<UpdatePositionAppointmentOrderRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdatePositionAppointmentOrderRegistrationDataRequest
            {
                DocumentId = s.Data.DocumentId,
            })
            .Send();
    }
}

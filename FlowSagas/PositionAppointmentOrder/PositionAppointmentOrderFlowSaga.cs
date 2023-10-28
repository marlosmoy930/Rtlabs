using ESD.Domain.Documents.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Holiday;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagas.FlowChainBuilding;
using FlowSagas.HolidayProcess;
using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;


namespace FlowSagas.PositionAppointmentOrder;

public class PositionAppointmentOrderFlowSaga : FlowSaga<PositionAppointmentOrderFlowSagaData,
        IPositionAppointmentOrderFlowContext>
{
    private readonly ILogger<PositionAppointmentOrderFlowSaga> _logger;

    public PositionAppointmentOrderFlowSaga(
        IServiceProvider serviceProvider,
        ILogger<PositionAppointmentOrderFlowSaga> logger)
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
                            .Then(Create.StartDocumentAcquaintanceUserInteractionSaga())))
            );
    }

    private bool WasAcquainted(FlowSpace<PositionAppointmentOrderFlowSagaData, IPositionAppointmentOrderFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool WasRegistered(FlowSpace<PositionAppointmentOrderFlowSagaData, IPositionAppointmentOrderFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<PositionAppointmentOrderFlowSagaData, IPositionAppointmentOrderFlowContext> UpdateOrderRegistrationData()
    {
        return Create.Monolog<UpdatePositionAppointmentOrderRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdatePositionAppointmentOrderRegistrationDataRequest
            {
                DocumentId = s.Data.DocumentId,
            })
            .Send();
    }
}

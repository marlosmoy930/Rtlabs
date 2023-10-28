using ESD.Domain.Documents.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagaContracts.PositionAppointmentOrderAfterDownsizingOrAbolishment;
using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.PositionAppointmentOrderAfterDownsizingOrAbolishment;

public class PositionAppointmentOrderAfterDownsizingOrAbolishmentFlowSaga : FlowSaga<PositionAppointmentOrderAfterDownsizingFlowSagaData,
        IPositionAppointmentOrderAfterDownsizingOrAbolishmentFlowContext>
{
    private readonly ILogger<PositionAppointmentOrderAfterDownsizingOrAbolishmentFlowSaga> _logger;

    public PositionAppointmentOrderAfterDownsizingOrAbolishmentFlowSaga(
        IServiceProvider serviceProvider,
        ILogger<PositionAppointmentOrderAfterDownsizingOrAbolishmentFlowSaga> logger)
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

    private bool WasRegistered(FlowSpace<PositionAppointmentOrderAfterDownsizingFlowSagaData, IPositionAppointmentOrderAfterDownsizingOrAbolishmentFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<PositionAppointmentOrderAfterDownsizingFlowSagaData, IPositionAppointmentOrderAfterDownsizingOrAbolishmentFlowContext> UpdateOrderRegistrationData()
    {
        return Create.Monolog<UpdatePositionAppointmentOrderRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdatePositionAppointmentOrderRegistrationDataRequest
            {
                DocumentId = s.Data.DocumentId,
            })
            .Send();
    }

    private bool WasAcquainted(FlowSpace<PositionAppointmentOrderAfterDownsizingFlowSagaData, IPositionAppointmentOrderAfterDownsizingOrAbolishmentFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }
}

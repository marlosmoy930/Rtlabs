using ESD.Domain.Documents.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointment;
using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;


namespace FlowSagas.PositionAppointment;

public class PositionAppointmentFlowSaga : FlowSaga<PositionAppointmentFlowSagaData,
        IPositionAppointmentFlowContext>
{
    private readonly ILogger<PositionAppointmentFlowSaga> _logger;

    public PositionAppointmentFlowSaga(IServiceProvider serviceProvider,
        ILogger<PositionAppointmentFlowSaga> logger) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        Create.StartApprovalSagaWithNotification()
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                Do(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
            );
   }
}

using ESD.Domain.Documents.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.AppointmentFromReserveStatement;
using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;


namespace FlowSagas.AppointmentFromReserveStatement;

public class AppointmentFromReserveStatementFlowSaga : FlowSaga<AppointmentFromReserveStatementFlowSagaData,
        IAppointmentFromReserveStatementFlowContext>
{
    private readonly ILogger<AppointmentFromReserveStatementFlowSaga> _logger;

    public AppointmentFromReserveStatementFlowSaga(IServiceProvider serviceProvider,
        ILogger<AppointmentFromReserveStatementFlowSaga> logger) : base(serviceProvider)
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

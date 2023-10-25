using ESD.Domain.Documents.Enums;
using FlowSagaContracts.AppointmentForTheTermOfOffice;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointment;
using FlowSagas.FlowChainBuilding;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;

namespace FlowSagas.AppointmentForTheTermOfOffice;

public class AppointmentForTheTermOfOfficeFlowSaga : FlowSaga<AppointmentForTheTermOfOfficeFlowSagaData, IAppointmentForTheTermOfOfficeFlowContext>
{
    public AppointmentForTheTermOfOfficeFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
    {
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
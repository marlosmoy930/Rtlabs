using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;

using FlowSagaContracts.AppointmentForPeriodOfEmployeeAbsence;
using FlowSagaContracts.AppointmentFromReserveOrder;
using FlowSagaContracts.Approving;
using FlowSagaContracts.PositionAppointmentOrder;
using FlowSagas.AppointmentFromReserveOrder;
using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.AppointmentForPeriodOfEmployeeAbsence;

public class AppointmentForPeriodOfEmployeeAbsenceFlowSaga
    : FlowSaga<AppointmentForPeriodOfEmployeeAbsenceFlowSagaData, IAppointmentForPeriodOfEmployeeAbsenceFlowContext>
{
    public AppointmentForPeriodOfEmployeeAbsenceFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
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
                        .Then(Create.SendDocumentRegisteredNotificationRequest())
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished)
                            .Then(Create.StartDocumentAcquaintanceUserInteractionSaga())))
                )
        );
    }


    private bool WasAcquainted(FlowSpace<AppointmentForPeriodOfEmployeeAbsenceFlowSagaData, IAppointmentForPeriodOfEmployeeAbsenceFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool IsStatementStage(FlowSpace<AppointmentForPeriodOfEmployeeAbsenceFlowSagaData, IAppointmentForPeriodOfEmployeeAbsenceFlowContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.AppointmentForPeriodOfEmployeeAbsenceStatement or
            StaffProcessStageCode.AppointmentForPeriodOfEmployeeAbsenceReplacementStatement or
            StaffProcessStageCode.AppointmentForPeriodOfEmployeeAbsenceReplacementStatementWithPresentation;
    }

    private bool IsOrderStage(FlowSpace<AppointmentForPeriodOfEmployeeAbsenceFlowSagaData, IAppointmentForPeriodOfEmployeeAbsenceFlowContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.AppointmentForPeriodOfEmployeeAbsenceOrder or
            StaffProcessStageCode.AppointmentForPeriodOfEmployeeAbsenceReplacementOrder;
    }

    private bool WasRegistered(FlowSpace<AppointmentForPeriodOfEmployeeAbsenceFlowSagaData, IAppointmentForPeriodOfEmployeeAbsenceFlowContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private FlowTaskChain<AppointmentForPeriodOfEmployeeAbsenceFlowSagaData, IAppointmentForPeriodOfEmployeeAbsenceFlowContext> UpdateOrderRegistrationData()
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

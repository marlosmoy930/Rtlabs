using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.ApproveStaffingTableStatement;
using FlowSagaContracts.Approving;
using FlowSagaContracts.Notification;
using FlowSagas.FlowChainBuilding;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;


namespace FlowSagas.ApproveStaffingTableStatementProcess;

public class ApproveStaffingTableStatementFlowSaga : FlowSaga<ApproveStaffingTableStatementFlowSagaData, 
    IApproveStaffingTableStatementFlowSagaContext>
{    
    private readonly IServiceProvider _serviceProvider;

    public ApproveStaffingTableStatementFlowSaga(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void ConfigureFlow()
    {        
         UpdateState(StaffProcessStageState.InProgress)
            .Then(Create.StartApprovalSagaWithNotification())
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected,
                UpdateState(StaffProcessStageState.Rejected)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review,
                UpdateState(StaffProcessStageState.Review)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected
                        && s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Review,
                Do(
                        UpdateState(StaffProcessStageState.Completed)
                    )
                    .Then(Create.StartDocumentRegistrationUserInteractionSaga())
                    .Then(Create.SendDocumentRegisteredNotificationRequest())
                    .Then(RegistrationUpdate())
                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                );
    }

  
    private FlowTaskChain<ApproveStaffingTableStatementFlowSagaData, IApproveStaffingTableStatementFlowSagaContext> UpdateState(StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateStateTasksForSagaRequest>();

        monolog.SetRequestFactory(s =>
        {
            return new UpdateStateTasksForSagaRequest()
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                State = state,
                DocumentId = s.Data.DocumentId
            };
        });

        return monolog.Send();
    }

    private FlowTaskChain<ApproveStaffingTableStatementFlowSagaData, IApproveStaffingTableStatementFlowSagaContext> RegistrationUpdate()
    {
        var monolog = Create.Monolog<RegistrationTaskForSagaRequest>();

        monolog.SetRequestFactory(s =>
        {
            return new RegistrationTaskForSagaRequest()
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                DocumentId = s.Data.DocumentId
            };
        });

        return monolog.Send();
    }
}
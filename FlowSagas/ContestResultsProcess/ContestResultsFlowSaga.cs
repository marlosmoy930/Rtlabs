using ESD.Domain.Documents.Enums; 
using ESD.Domain.StaffProcesses.Enums;
using FlowSagaContracts.Approving;
using FlowSagaContracts.ContestResults;
using FlowSagas.FlowChainBuilding;
using Microsoft.Extensions.Logging;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.ContestResultsProcess;
 
public class ContestResultsFlowSaga : FlowSaga<ContestResultsFlowSagaData, IContestResultsFlowSagaContext>
{
    private readonly ILogger<ContestResultsFlowSagaData> _logger;

    public ContestResultsFlowSaga(ILogger<ContestResultsFlowSagaData> logger,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        UpdateContestResultsStatementState(StaffProcessStageState.InProgress)
            .Then(Create.StartApprovalSagaWithNotification())
            .ThenIf(WasRejected, UpdateContestResultsStatementState(StaffProcessStageState.Rejected))
            .ThenIf(WasSentToReview, UpdateContestResultsStatementState(StaffProcessStageState.Review))
            .ThenIf(WasCompleted,
                UpdateContestResultsStatementState(StaffProcessStageState.Completed)
                    .Then(Create.SendDocumentApprovedNotificationRequest())
                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
            ); 
    }
     
    private bool WasCompleted(FlowSpace<ContestResultsFlowSagaData, IContestResultsFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected && 
               s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Review;
    }

    private bool WasSentToReview(FlowSpace<ContestResultsFlowSagaData, IContestResultsFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review;
    }

    private bool WasRejected(FlowSpace<ContestResultsFlowSagaData, IContestResultsFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected;
    }
     
    private FlowTaskChain<ContestResultsFlowSagaData, IContestResultsFlowSagaContext> UpdateContestResultsStatementState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateContestResultStateRequest>();

        monolog.SetRequestFactory(s => new UpdateContestResultStateRequest
        {
            ApprovalSagaCorrelationId = s.CorrelationId,
            State = state,
            DocumentId = s.Data.DocumentId
        });

        return monolog.Send();
    }  
}

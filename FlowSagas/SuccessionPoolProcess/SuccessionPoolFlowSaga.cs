using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving;  
using FlowSagaContracts.SuccessionPool;
using FlowSagas.FlowChainBuilding; 
using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.SuccessionPoolProcess;
 
public class SuccessionPoolFlowSaga : FlowSaga<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext>
{
    private readonly ILogger<SuccessionPoolFlowSagaData> _logger;

    public SuccessionPoolFlowSaga(ILogger<SuccessionPoolFlowSagaData> logger,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        DoIf(
            IsConsentStage,
            UpdateSuccessionPoolConsentState(StaffProcessStageState.InProgress)
                .Then(Create.StartApprovalSagaWithNotification())
                .ThenIf(WasRejected, UpdateSuccessionPoolConsentState(StaffProcessStageState.Rejected))
                .ThenIf(WasSentToReview, UpdateSuccessionPoolConsentState(StaffProcessStageState.Review))
                .ThenIf(WasCompleted,
                    UpdateSuccessionPoolConsentState(StaffProcessStageState.Completed)
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Signed)) 
                )   
        );

        DoIf(
            IsOrderStage,
            UpdateSuccessionPoolOrderState(StaffProcessStageState.InProgress)
                .Then(Create.StartApprovalSagaWithNotification())
                .ThenIf(WasRejected, UpdateSuccessionPoolOrderState(StaffProcessStageState.Rejected))
                .ThenIf(WasSentToReview, UpdateSuccessionPoolOrderState(StaffProcessStageState.Review))
                .ThenIf(WasCompleted, UpdateSuccessionPoolOrderState(StaffProcessStageState.Completed)
                    .Then(Create.StartDocumentRegistrationUserInteractionSaga()
                            .ThenIf(WasRegistered, UpdateSuccessionPoolOrderRegistrationData()  
                                .Then(Create.SendDocumentRegisteredNotificationRequest())
                                .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                                .Then(SendSuccessionPoolInclusionNotification())  
                            )
                    )
                ) 
        );
    }

    private bool WasRegistered(FlowSpace<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }
      
    private bool WasCompleted(FlowSpace<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected && 
               s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Review;
    }

    private bool WasSentToReview(FlowSpace<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review;
    }

    private bool WasRejected(FlowSpace<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected;
    }

    private bool IsConsentStage(FlowSpace<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.SuccessionPoolConsent;
    }

    private bool IsOrderStage(FlowSpace<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> s)
    {
        return s.Data.StageCode is 
            StaffProcessStageCode.SuccessionPoolOrder or 
            StaffProcessStageCode.SuccessionPoolMassOrder;
    } 

    private FlowTaskChain<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> UpdateSuccessionPoolConsentState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateSuccessionPoolConsentStateRequest>();

        monolog.SetRequestFactory(s => new UpdateSuccessionPoolConsentStateRequest
        {
            ApprovalSagaCorrelationId = s.CorrelationId,
            State = state,
            DocumentId = s.Data.DocumentId
        });

        return monolog.Send();
    }

    private FlowTaskChain<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> UpdateSuccessionPoolOrderState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateSuccessionPoolOrderStateRequest>();

        monolog.SetRequestFactory(s => new UpdateSuccessionPoolOrderStateRequest
        {
            ApprovalSagaCorrelationId = s.CorrelationId,
            State = state,
            DocumentId = s.Data.DocumentId
        });

        return monolog.Send();
    }

    private FlowTaskChain<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> UpdateSuccessionPoolOrderRegistrationData()
    {
        return Create.Monolog<UpdateSuccessionPoolOrderRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdateSuccessionPoolOrderRegistrationDataRequest
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                DocumentId = s.Data.DocumentId
            })
            .Send();
    }
     
    private FlowTaskChain<SuccessionPoolFlowSagaData, ISuccessionPoolFlowSagaContext> SendSuccessionPoolInclusionNotification()
    {
        return Create.Monolog<UpdateSuccessionPoolInclusionRequest>()
            .SetRequestFactory(s => new UpdateSuccessionPoolInclusionRequest()
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                DocumentId = s.Data.DocumentId
            })
            .Send();
    }
} 
using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving; 
using FlowSagaContracts.Holiday;
using FlowSagaContracts.Notification;
using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Spaces;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.HolidayProcess;
 
public class EduHolidayFlowSaga : FlowSaga<EduHolidayFlowSagaData, IHolidayFlowSagaContext>
{
    private readonly ILogger<EduHolidayFlowSaga> _logger;

    public EduHolidayFlowSaga(ILogger<EduHolidayFlowSaga> logger,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        DoIf(
            IsStatementStage,
            UpdateHolidayStatementState(StaffProcessStageState.InProgress)
                .Then(Create.StartApprovalSagaWithNotification())
                .ThenIf(WasRejected, UpdateHolidayStatementState(StaffProcessStageState.Rejected))
                .ThenIf(WasSentToReview, UpdateHolidayStatementState(StaffProcessStageState.Review))
                .ThenIf(WasCompleted,
                    UpdateHolidayStatementState(StaffProcessStageState.Completed)
                        .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Signed))
                        .Then(SendHolidayStatementApprovedNotification())
                )   
        );

        DoIf(
            IsOrderStage,
            UpdateHolidayOrderState(StaffProcessStageState.InProgress)
                .Then(Create.StartApprovalSagaWithNotification())
                .ThenIf(WasRejected, UpdateHolidayOrderState(StaffProcessStageState.Rejected))
                .ThenIf(WasSentToReview, UpdateHolidayOrderState(StaffProcessStageState.Review))
                .ThenIf(WasCompleted,
                    UpdateHolidayOrderState(StaffProcessStageState.Completed)
                        .Then(Create.StartDocumentRegistrationUserInteractionSaga()
                            .ThenIf(WasRegistered, UpdateHolidayOrderRegistrationData())
                            .Then(Create.SendDocumentRegisteredNotificationRequest()
                                .Then(Create.StartDocumentAcquaintanceUserInteractionSaga()
                                    .ThenIf(WasAcquainted, UpdateHolidayOrderAcquaintanceDate())
                                    .Then(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished)) 
                                )
                            )
                        )
                )
        );
    }

    private bool WasRegistered(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Registered";
    }

    private bool WasAcquainted(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.UserInteractionSagaResult == "Acquainted";
    }

    private bool WasCompleted(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected && 
               s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Review;
    }

    private bool WasSentToReview(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Review;
    }

    private bool WasRejected(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected;
    }

    private bool IsStatementStage(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.StageCode is
            StaffProcessStageCode.EduHolidayWithPayStatement or
            StaffProcessStageCode.EduHolidayWithoutPayStatement;
    }

    private bool IsOrderStage(FlowSpace<EduHolidayFlowSagaData, IHolidayFlowSagaContext> s)
    {
        return s.Data.StageCode is 
            StaffProcessStageCode.EduHolidayWithPayOrder or 
            StaffProcessStageCode.EduHolidayWithoutPayOrder;
    } 

    private FlowTaskChain<EduHolidayFlowSagaData, IHolidayFlowSagaContext> UpdateHolidayStatementState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateHolidayStatementStateRequest>();

        monolog.SetRequestFactory(s => new UpdateHolidayStatementStateRequest
        {
            ApprovalSagaCorrelationId = s.CorrelationId,
            State = state,
            DocumentId = s.Data.DocumentId
        });

        return monolog.Send();
    }

    private FlowTaskChain<EduHolidayFlowSagaData, IHolidayFlowSagaContext> UpdateHolidayOrderState(
        StaffProcessStageState state)
    {
        var monolog = Create.Monolog<UpdateHolidayOrderStateRequest>();

        monolog.SetRequestFactory(s => new UpdateHolidayOrderStateRequest
        {
            ApprovalSagaCorrelationId = s.CorrelationId,
            State = state,
            DocumentId = s.Data.DocumentId
        });

        return monolog.Send();
    }

    private FlowTaskChain<EduHolidayFlowSagaData, IHolidayFlowSagaContext> UpdateHolidayOrderRegistrationData()
    {
        return Create.Monolog<UpdateHolidayOrderRegistrationDataRequest>()
            .SetRequestFactory(s => new UpdateHolidayOrderRegistrationDataRequest
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                DocumentId = s.Data.DocumentId
            })
            .Send();
    }

    private FlowTaskChain<EduHolidayFlowSagaData, IHolidayFlowSagaContext> SendHolidayStatementApprovedNotification()
    {
        return Create.Monolog<NotificationHolidayStatementApprovedRequest>()
            .SetRequestFactory(s => new NotificationHolidayStatementApprovedRequest
            {
                UserId = s.UserId.Value,
                DocumentId = s.Data.DocumentId,
            })
            .Send();
    }

    private FlowTaskChain<EduHolidayFlowSagaData, IHolidayFlowSagaContext> UpdateHolidayOrderAcquaintanceDate()
    {
        return Create.Monolog<UpdateHolidayOrderAcquaintanceDateRequest>()
            .SetRequestFactory(s => new UpdateHolidayOrderAcquaintanceDateRequest()
            {
                ApprovalSagaCorrelationId = s.CorrelationId,
                DocumentId = s.Data.DocumentId
            })
            .Send();
    }
}

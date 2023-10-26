using ESD.Domain.Documents.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Document;
using FlowSagaContracts.Notification;
using FlowSagaContracts.StaffProcessEvent;
using FlowSagaContracts.UserInteraction;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Communicators;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

namespace FlowSagas.FlowChainBuilding;

public static class FlowChainBuilder
{
    public static FlowTaskChain<TFlowData, TSagaContext> StartApprovalSaga<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory)
        where TFlowData : class, IFlowSagaDataBase, new()
        where TSagaContext : class
    {
        return flowFactory.ChildSaga<ApprovalSagaData, IApprovalSagaContext>("Approval Saga")
            .SetRequestFactory(s =>
                ApprovalSagaData.Create(
                    s.CurrentSagaSpace.Data.ApprovalChain.ApprovalSteps.ToList(),
                    s.CurrentSagaSpace.Data.DocumentId,
                    s.CurrentSagaSpace.Data.ApprovalChain.StaffProcessStageId,
                    s.CurrentSagaSpace.Data.DepartmentId,
                    s.CurrentSagaSpace.Data.TaskNameTemplateSubstitutions,
                    s.CurrentSagaSpace.Data.AdditionalData,
                    s.CurrentSagaSpace.Data.StageCode
                ))
            .SetChunkResponseProcessor((s, chunkIndex) =>
            {
                s.CurrentSagaSpace.Data.ApprovalSagaResultType = s.ChildSagaSpace.Data.ApprovalSagaResultType;
            })
            .Start();
    }
      
    public static FlowTaskChain<TFlowData, TSagaContext> StartApprovalSagaWithNotification<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory)
        where TFlowData : class, IFlowSagaDataBase, new()
        where TSagaContext : class
    {
       return flowFactory
                .Monolog<StaffProcessEventRequest>()
                .SetRequestFactory(s =>
                    new StaffProcessEventRequest
                    {
                        ApprovalSagaInstanceId = s.CorrelationId,
                        DocumentId = s.Data.DocumentId,
                        DateTimeUtc = DateTime.UtcNow,
                        StaffProcessEventType = StaffProcessEventType.StaffProcessStarted,
                        UserId = s.UserId
                    })
                .Send()
            .Then(
                flowFactory.StartApprovalSaga()
                );
    }

    public static FlowTaskChain<TFlowData, TSagaContext> StartDocumentRegistrationUserInteractionSaga<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory)
        where TFlowData : class, IFlowSagaWithUserInteractionSagaData, new()
        where TSagaContext : class
    {
        return flowFactory.ChildSaga<UserInteractionSagaData, IApprovalSagaContext>("User Interaction Saga")
            .SetRequestFactory(s =>
                new UserInteractionSagaData
                {
                    DocumentId = s.CurrentSagaSpace.Data.DocumentId,
                    DepartmentId = s.CurrentSagaSpace.Data.DepartmentId,
                    NeedsRegistration = true,
                    StageId = s.CurrentSagaSpace.Data.ApprovalChain.StaffProcessStageId,
                }
            )
            .SetChunkResponseProcessor((s, chunkIndex) =>
            {
                s.CurrentSagaSpace.Data.UserInteractionSagaResult = s.ChildSagaSpace.Data.EndData;
            })
            .Start();
    }

    public static FlowTaskChain<TFlowData, TSagaContext> StartDocumentAcquaintanceUserInteractionSaga<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory)
        where TFlowData : class, IFlowSagaWithUserInteractionSagaData, new()
        where TSagaContext : class
    {
        return flowFactory.ChildSaga<UserInteractionSagaData, IApprovalSagaContext>("User Interaction Saga")
            .SetRequestFactory(s =>
                new UserInteractionSagaData
                {
                    DocumentId = s.CurrentSagaSpace.Data.DocumentId,
                    StageId = s.CurrentSagaSpace.Data.ApprovalChain.StaffProcessStageId,
                    NeedsAcquaintance = true
                }
            )
            .SetChunkResponseProcessor((s, chunkIndex) =>
            {
                s.CurrentSagaSpace.Data.UserInteractionSagaResult = s.ChildSagaSpace.Data.EndData;
            })
            .Start();
    }
    
    public static FlowTaskChain<TFlowData, TSagaContext> SendDocumentRegisteredNotificationRequest<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory)
        where TFlowData : class, IFlowSagaDataBase, new()
        where TSagaContext : class
    {
        return flowFactory
            .Monolog<NotificationDocumentRegisteredRequest>()
            .SetRequestFactory(s =>
                new NotificationDocumentRegisteredRequest
                {
                    ApprovalSagaCorrelationId = s.CorrelationId,
                    DocumentId = s.Data.DocumentId
                })
            .Send();
    }

    public static FlowTaskChain<TFlowData, TSagaContext> SendDocumentApprovedNotificationRequest<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory)
        where TFlowData : class, IFlowSagaDataBase, new()
        where TSagaContext : class
    {
        return flowFactory
            .Monolog<NotificationDocumentApprovedRequest>()
            .SetRequestFactory(s =>
                new NotificationDocumentApprovedRequest
                {
                    ApprovalSagaCorrelationId = s.CorrelationId,
                    DocumentId = s.Data.DocumentId
                })
            .Send();
    }

    public static FlowTaskChain<TFlowData, TSagaContext> SendSetDocumentStatusRequest<TFlowData, TSagaContext>(
        this FlowCommunicatorFactory<TFlowData, TSagaContext> flowFactory,
        DocumentStatus documentStatus)
        where TFlowData : class, IFlowSagaDataBase, new()
        where TSagaContext : class
    {
        return flowFactory
            .Monolog<SetDocumentStatusRequest>()
            .SetRequestFactory(s =>
                new SetDocumentStatusRequest
                {
                    SagaId = s.CorrelationId,
                    DocumentId = s.Data.DocumentId,
                    DocumentStatus = documentStatus
                })
            .Send();
    }
}

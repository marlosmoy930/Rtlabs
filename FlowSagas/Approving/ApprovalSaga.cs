using Common.Exceptions;

using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;
using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.ApprovalChainModification;
using FlowSagaContracts.ApprovalTask;
using FlowSagaContracts.Approving;
using FlowSagaContracts.Approving.ApprovalChain;
using FlowSagaContracts.Document;
using FlowSagaContracts.StaffProcessEvent;
using FlowSagaContracts.StaffProcessSubscriber;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Messages;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Utils;
using RTLabs.EisUks.FlowEngine.MassTransit.Logging.Helpers;
using RTLabs.EisUks.FlowEngine.MassTransit.Settings.Configuration;
using Services.Models.DocumentAttachments;
using Services.Utils;

namespace FlowSagas.Approving;

public class ApprovalSaga : MassTransitStateMachine<ApprovalSagaInstance>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _rabbitMqConnectionString;
    private static Event<FlowSagaStartCommand<ApprovalSagaData>> StartSagaEvent { get; set; }
    private static Event<ApprovalActionData> ApprovalEvent { get; set; }
    private static Event<ApprovalChainModificationData> ApprovalChainModifiedEvent { get; set; }
    private static Event<FlowSagaEndCommand> EndSagaEvent { get; set; }

    public State Launched { get; set; }

    public ApprovalSaga(
        IServiceProvider serviceProvider,
        ILogger<ApprovalSaga> logger,
        RabbitMqConnectionStringProvider rabbitMqConnectionStringProvider)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqConnectionString = rabbitMqConnectionStringProvider.ConnectionString;
        InstanceState(x => x.CurrentState);
        Event(() => StartSagaEvent, x =>
        {
            x.CorrelateById(ctx => ctx.Message.CorrelationId);
            x.InsertOnInitial = true;
        });
        Event(() => ApprovalEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => ApprovalChainModifiedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => EndSagaEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Initially(
            When(StartSagaEvent)
                .ThenAsync(ProcessStartSagaEventAsync)
                .TransitionTo(Launched)
        );

        During(Launched,
            When(ApprovalEvent)
                .ThenAsync(PerformApprovalActionAsync)
        );

        During(Launched,
            When(ApprovalChainModifiedEvent)
                .Then(PerformApprovalChainModified)
        );

        During(Launched,
            When(EndSagaEvent)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    private async Task ProcessStartSagaEventAsync(BehaviorContext<ApprovalSagaInstance, FlowSagaStartCommand<ApprovalSagaData>> ctx)
    {
        var correlationLogManager = CorrelationUtil.GetCorrelationLogManagerFromContextHeaders(ctx);

        var instance = ctx.Saga;
        var command = ctx.Message;

        instance.UserId = command.UserId;
        instance.UserEmail = command.UserEmail;
        instance.StartedAt = command.StartedAt;
        instance.LogCorrelationId = correlationLogManager.LogCorrelationId;
        instance.ParentSagaAddress = ctx.SourceAddress!.AbsoluteUri;
        instance.ParentSagaCorrelationId = command.ParentSagaCorrelationId!.Value;
        instance.ParentSagaFlowTaskId = command.TaskId!.Value;
        instance.DocumentId = command.FlowData.DocumentId;
        instance.StaffProcessStageId = command.FlowData.StaffProcessStageId; 
        instance.ApprovalDataJson = command.FlowData.ApprovalDataJson;
        
        var firstStep = instance.GetSteps().First();
        firstStep.Index = 0;

        var changeApprovalTasksRequest = 
            new ChangeApprovalTasksRequest
            {
                DocumentId = command.FlowData.DocumentId,
                StageCode = ctx.Saga.ApprovalData.StageCode,
                CreateApprovalTasksRequest = GetCreateApprovalTasksRequest(firstStep, instance),
                TaskResolvedData = null,
                SubjectUserId = instance.ApprovalData.SubjectId,
                
            };
        await ChangeApprovalTasksAsync(ctx, changeApprovalTasksRequest);
        await SetDocumentStatusAsync(ctx, firstStep);
    }

    private void PerformApprovalChainModified(BehaviorContext<ApprovalSagaInstance, ApprovalChainModificationData> ctx)
    {
        var updatedSagaSteps = ctx.Message.Steps;
        var currentStepIndex = ctx.Saga.CurrentStepIndex;
        var sagaSteps = ctx.Saga.GetSteps();

        for (var i = 0; i <= currentStepIndex; i++)
        {
            updatedSagaSteps[i] = sagaSteps[i];
        }

        ctx.Saga.SetSteps(updatedSagaSteps);
        /*
            // Логика на случай возможности изменения задач текущего шага саги
                var updatedApprovalStep = updatedSagaSteps[currentStepIndex];
                var currentApprovalStep = currentSagaSteps[currentStepIndex];
                currentApprovalStep.Index = currentStepIndex;  

                await SendFlowRequestAsync(
                    ctx,
                    new ApprovalChainModificationDataEventRequest
                    {
                        ApprovalSagaCorrelationId = ctx.Saga.CorrelationId,
                        DocumentId = ctx.Saga.DocumentId,
                        CurrentApprovalStep = currentApprovalStep,
                        StaffProcessStageId = ctx.Saga.StaffProcessStageId,
                        TaskNameTemplateSubstitutions = ctx.Saga.GetTaskNameTemplateSubstitutions(),

                        PerformedUtc = ctx.Message.PerformedUtc,
                        UpdatedApprovalStep = updatedApprovalStep, 
                    }
                );
        */
    }

    private async Task PerformApprovalActionAsync(BehaviorContext<ApprovalSagaInstance, ApprovalActionData> ctx)
    {
        var approvalActionData = ctx.Message;
        if (approvalActionData.ActionType == ApprovalActionType.Return)
        {
            await ReturnOneStepBackAsync(ctx);
            return;
        }

        var currentStepIndex = ctx.Saga.CurrentStepIndex;
        var steps = ctx.Saga.GetSteps();
        var resolvedStep = steps[currentStepIndex];

        resolvedStep.SetResult(approvalActionData);

        if (resolvedStep.Result is null)
        {
            return;
        }

        ctx.Saga.SetSteps(steps);


        await SendFlowRequestAsync(
            ctx,
            new ApprovalTaskResolvedEventRequest
            {
                ApprovalTaskId = ctx.Message.ApprovalTaskId,
                ActionType = ctx.Message.ActionType,
                ApprovalTaskType = resolvedStep.ApprovalType,
                DocumentId = ctx.Message.DocumentId,
                UserId = ctx.Message.AssigneeId,
                UserName = ctx.Message.AssigneeLogin,
                DateTimeUtc = ctx.Message.PerformedUtc,
            }
        );

        ApprovalSagaStep newStep = null;
        if (resolvedStep.Result == ApprovalSagaStepResultType.Rejected)
        {
            await SendResponseToParentSagaAsync(ctx, resolvedStep.Result!.Value);
            await SendSetDocumentStatusAsync(DocumentStatus.Rejected, ctx);
        }
        else if (resolvedStep.Result == ApprovalSagaStepResultType.Review)
        {
            await SendResponseToParentSagaAsync(ctx, resolvedStep.Result!.Value);
            await SendSetDocumentStatusAsync(DocumentStatus.OnRefinement, ctx);
        }
        else if (resolvedStep.Result == ApprovalSagaStepResultType.Canceled)
        {
            await SendResponseToParentSagaAsync(ctx, resolvedStep.Result!.Value);
            await SendSetDocumentStatusAsync(DocumentStatus.Canceled, ctx);
        }
        else
        {
            if (ctx.Message.ActionType == ApprovalActionType.ApproveAndSign
                && (ctx.Message.SignedDocumentWithAttachments != null
                    || ctx.Message.DocumentAttachmentSignature != null))
            {
                SignedUserType? format = null;
                 
                var stageCode = ctx.Saga.ApprovalData.StageCode;
         
                if (stageCode == StaffProcessStageCode.SuccessionPoolConsent)
                {
                    format = SignedUserType.SuccessionPoolConsent;
                }

                if (NgdProcessHelper.IsNgdProcessPartTwo((int)stageCode))
                {
                    format = SignedUserType.ContractEmployer;
                }

                if (NgdProcessHelper.IsNgdProcessPartOne(stageCode))
                {
                    format = SignedUserType.Short;
                }

                if (ctx.Message.StampFormat != null)
                {
                    format = ctx.Message.StampFormat;
                }

                if (ctx.Message.SignedDocumentWithAttachments != null)
                {
                    await SendFlowRequestAsync(
                        ctx,
                        new SaveDocumentAndAttachmentSignaturesRequest
                        {
                            CreatedUtc = ctx.Message.PerformedUtc,
                            SignedDocumentDataAndUserAttachments = ctx.Message.SignedDocumentWithAttachments,
                            SignerId = ctx.Message.AssigneeId,
                            SignerLogin = ctx.Message.AssigneeLogin,
                            Format = format ?? SignedUserType.Full
                        }
                    );
                }
                else
                {
                    await SendFlowRequestAsync(
                        ctx,
                        new SaveDocumentAttachmentSignatureRequest
                        {
                            CreatedUtc = ctx.Message.PerformedUtc,
                            DocumentAttachmentSignature = ctx.Message.DocumentAttachmentSignature,
                            SignerId = ctx.Message.AssigneeId,
                            SignerLogin = ctx.Message.AssigneeLogin,
                            Format = format ?? SignedUserType.Full
                        }
                    );
                }

                await SendFlowRequestAsync(
                    ctx,
                    new RemoveStaffProcessSubscriberRequest
                    {
                        CorrelationId = ctx.Message.CorrelationId,
                        UserId = ctx.Message.AssigneeId
                    }
                );
            }

            if (resolvedStep.Result == ApprovalSagaStepResultType.Approved)
            {
                var newStepIndex = currentStepIndex + 1;
                ctx.Saga.CurrentStepIndex = newStepIndex;

                if (ctx.Saga.CurrentStepIndex < steps.Count)
                {
                    newStep = steps[newStepIndex];
                    newStep.Index = newStepIndex;
                    await SetDocumentStatusAsync(ctx, newStep, resolvedStep);
                }
                else
                {
                    await SendResponseToParentSagaAsync(ctx, resolvedStep.Result!.Value);
                }
            }
        }

        await ChangeApprovalTasksAsync(
            ctx, 
            new ChangeApprovalTasksRequest
            {
                DocumentId = ctx.Message.DocumentId,
                StageCode = ctx.Saga.ApprovalData.StageCode,
                CreateApprovalTasksRequest = 
                    newStep == null
                        ? null
                        : GetCreateApprovalTasksRequest(newStep, ctx.Saga),
                TaskResolvedData = GetTaskResolvedData(ctx, resolvedStep, currentStepIndex, steps.Count)
            });
        
        await SendFlowRequestAsync(
            ctx,
            new ResolveApprovalTaskRequest
            {
                ApprovalAction = ctx.Message.ActionType,
                ApprovalTaskId = ctx.Message.ApprovalTaskId,
                ResolvedUtc = ctx.Message.PerformedUtc
            }
        );
    }

    private static ApprovalTaskResolvedData GetTaskResolvedData(
        BehaviorContext<ApprovalSagaInstance, ApprovalActionData> ctx, 
        ApprovalSagaStep resolvedStep, 
        int resolvedStepIndex, 
        int stepsCount)
    {
        return new ApprovalTaskResolvedData
        {
            ApprovalTaskId = ctx.Message.ApprovalTaskId,
            ActionType = ctx.Message.ActionType,
            ResolvedStepApprovalType = resolvedStep.ApprovalType,
            UserId = ctx.Message.AssigneeId,
            UserName = ctx.Message.AssigneeLogin,
            DateTimeUtc = ctx.Message.PerformedUtc,
            ResolvedStepResult = resolvedStep.Result,
            ResolvedStepIndex = resolvedStepIndex,
            TotalSteps = stepsCount,
            SagaOwnerId = ctx.Saga.UserId,
            Comment = ctx.Message.Comment,
            IsGovernmentKeyApprovalProcessExternalTask = ctx.Message.IsGovernmentKeyApprovalProcessExternalAction
        };
    }

    private async Task ReturnOneStepBackAsync(BehaviorContext<ApprovalSagaInstance, ApprovalActionData> ctx)
    {
        var saga = ctx.Saga;
        var currentStepIndex = saga.CurrentStepIndex;
        if (currentStepIndex == 0)
        {
            throw new BusinessException("Возврат к предыдущему шагу невозможен, текущий шаг является первым в маршруте согласования");
        }

        var steps = saga.GetSteps();
        var resolvedStep = steps[currentStepIndex];

        if (resolvedStep.ApprovalType != ApprovalType.CheckWithReturnOnFail)
        {
            throw new ArgumentException($"Недопустимое действие {ApprovalActionType.Return} для шага с типом {resolvedStep.ApprovalType}");
        }

        resolvedStep.ApprovedAssignees.Clear();

        var dateTimeUtc = ctx.Message.PerformedUtc;
        var excludedTaskId = ctx.Message.ApprovalTaskId;

        await SendFlowRequestAsync(
            ctx,
            new DeleteApprovalTasksForSagaStepRequest
            {
                ApprovalSagaCorrelationId = ctx.Saga.CorrelationId,
                PerformedUtc = dateTimeUtc,
                ApprovalStepIndex = currentStepIndex,
                ExcludedTaskId = excludedTaskId
            }
        );
        
        var newStepIndex = currentStepIndex - 1;
        saga.CurrentStepIndex = newStepIndex;
        var newCurrentStep = steps[newStepIndex];
        newCurrentStep.ApprovedAssignees.Clear();
        newCurrentStep.Result = null;
        newCurrentStep.IsReturnedOnCheckFail = true;

        ctx.Saga.SetSteps(steps);

        var approvalData = ctx.Saga.ApprovalData;
        

        var comment = ctx.Data.Comment;
        if (!string.IsNullOrEmpty(comment))
        {
            if (approvalData.AdditionalData == null)
            {
                approvalData.AdditionalData = new Dictionary<FlowSagaAdditionalDataKey, string>();
            }
            approvalData.AdditionalData[FlowSagaAdditionalDataKey.DialogBoxMessage] = comment;
            ctx.Saga.ApprovalData = approvalData;
        }
       
        await ChangeApprovalTasksAsync(
            ctx, 
            new ChangeApprovalTasksRequest
            {
                DocumentId = ctx.Message.DocumentId,
                StageCode = ctx.Saga.ApprovalData.StageCode,
                CreateApprovalTasksRequest = GetCreateApprovalTasksRequest(newCurrentStep, ctx.Saga),
                TaskResolvedData = GetTaskResolvedData(ctx, resolvedStep, currentStepIndex, steps.Count),
            });

        await SendFlowRequestAsync(
            ctx,
            new ApprovalTaskResolvedEventRequest
            {
                ActionType = ctx.Message.ActionType,
                ApprovalTaskId = ctx.Message.ApprovalTaskId,
                DocumentId = ctx.Message.DocumentId,
                UserId = ctx.Message.AssigneeId,
                UserName = ctx.Message.AssigneeLogin,
                DateTimeUtc = ctx.Message.PerformedUtc,
            }
        );

        await SendFlowRequestAsync(
            ctx,
            new ResolveApprovalTaskRequest
            {
                ApprovalAction = ctx.Message.ActionType,
                ApprovalTaskId = ctx.Message.ApprovalTaskId,
                ResolvedUtc = ctx.Message.PerformedUtc
            }
        );

        await SendSetDocumentStatusAsync(DocumentStatus.DocumentsRequired, ctx);
    }

    private async Task ChangeApprovalTasksAsync(
        SagaConsumeContext<ApprovalSagaInstance> ctx, ChangeApprovalTasksRequest changeTasksRequest)
    {
        var correlationLogManager = CorrelationUtil.GetCorrelationLogManagerFromContextHeaders(ctx);
        var instance = ctx.Saga;

        var flowSagaService = _serviceProvider.GetRequiredService<IFlowSagaService>();
        await flowSagaService.SendFlowRequestAsync(correlationLogManager, changeTasksRequest, instance.StartedAt);
    }

    private static CreateApprovalTasksRequest GetCreateApprovalTasksRequest(ApprovalSagaStep step,
        ApprovalSagaInstance instance)
    {
        var approvalData = instance.ApprovalData;
        var data = new CreateApprovalTasksRequest
        {
            ApprovalSagaCorrelationId = instance.CorrelationId,
            InitiatorId = instance.UserId.Value,
            DepartmentId = approvalData.DepartmentId,
            SubjectId = approvalData.SubjectId,
            CommissionMeetingId = approvalData.CommissionMeetingId,
            DocumentId = instance.DocumentId,
            ApprovalStep = step,
            StaffProcessStageId = instance.StaffProcessStageId,
            TaskNameTemplateSubstitutions = approvalData.TaskNameTemplateSubstitutions,
            AdditionalData = approvalData.AdditionalData
        };
        return data;
    }

    private async Task SendResponseToParentSagaAsync(
        SagaConsumeContext<ApprovalSagaInstance, ApprovalActionData> ctx,
        ApprovalSagaStepResultType result)
    {
        var instance = ctx.Saga;
        var response = GetParentSagaResponse(instance,
            new ApprovalSagaData
            {
                ApprovalSagaResultType = result,
            }
        );

        var endpoint = await ctx.GetSendEndpoint(new Uri(instance.ParentSagaAddress!));

        var faultAddress = FlowQueueUtil.GetUnifiedFaultAddress(_rabbitMqConnectionString);
        await endpoint.Send(response, c => { c.FaultAddress = faultAddress; });

        var dateTimeUtc = ctx.Message.PerformedUtc;
        var excludedTaskId = ctx.Message.ApprovalTaskId;

        await SendFlowRequestAsync(
            ctx,
            new DeleteApprovalTasksForSagaRequest
            {
                ApprovalSagaCorrelationId = ctx.Saga.CorrelationId,
                PerformedUtc = dateTimeUtc,
                ApprovalSteps = ctx.Saga.GetSteps(),
                ExcludedTaskId = excludedTaskId
            }
        );

        await SendFlowRequestAsync(
            ctx,
            new StaffProcessEventRequest
            {
                ApprovalSagaInstanceId = instance.CorrelationId,
                DateTimeUtc = dateTimeUtc,
                DocumentId = instance.DocumentId,
                StaffProcessEventType = StaffProcessEventType.StaffProcessTerminated
            }
        );
    }

    private async Task SendSetDocumentStatusAsync<TSaga, TMessage>(DocumentStatus status, SagaConsumeContext<TSaga, TMessage> ctx)
        where TSaga : ApprovalSagaInstance, ISaga
        where TMessage : class
    {
        var instance = ctx.Saga;

        await SendFlowRequestAsync(
            ctx,
            new SetDocumentStatusRequest
            {
                SagaId = instance.CorrelationId,
                DocumentId = instance.DocumentId,
                DocumentStatus = status
            }
        );
    }

    private async Task SetDocumentStatusAsync<TSaga, TMessage>(
            SagaConsumeContext<TSaga, TMessage> ctx,
            ApprovalSagaStep currentStep,
            ApprovalSagaStep previousStep = null) 
        where TSaga : ApprovalSagaInstance, ISaga
        where TMessage : class
    {
        if (previousStep == null)
        {
            var documentStatus = currentStep.ApprovalType == ApprovalType.Sign
                ? DocumentStatus.OnSign
                : DocumentStatus.OnApproval;

            await SendSetDocumentStatusAsync(documentStatus, ctx);
        }
        else
        {
            if (currentStep.ApprovalType == ApprovalType.Sign
                && previousStep.ApprovalType == ApprovalType.Approve)
            {
                await SendSetDocumentStatusAsync(DocumentStatus.OnSign, ctx);
            }
            else if (currentStep.ApprovalType == ApprovalType.Approve
                && previousStep.ApprovalType == ApprovalType.Sign)
            {
                await SendSetDocumentStatusAsync(DocumentStatus.OnApproval, ctx);
            }
            else if (currentStep.ApprovalType == ApprovalType.CheckWithReturnOnFail)
            {
                await SendSetDocumentStatusAsync(DocumentStatus.OnDocumentsChecking, ctx);
            }
        }
    }

    private static FlowResponse GetParentSagaResponse(ApprovalSagaInstance instance, ApprovalSagaData approvalResponse)
    {
        const int parentSagaChunkIndex = 0;
        var responseData = new ChildSagaFlowResponseData<ApprovalSagaData>
        {
            CorrelationId = instance.CorrelationId,
            FlowData = approvalResponse
        };

        var response = new FlowResponse
        {
            CorrelationId = instance.ParentSagaCorrelationId,
            TaskId = instance.ParentSagaFlowTaskId,
            ChunkIndex = parentSagaChunkIndex,
            JsonResult = JsonConvert.SerializeObject(responseData)
        };
        
        return response;
    }

    private async Task SendFlowRequestAsync<TRequest>(
        ConsumeContext ctx,
        TRequest request)
        where TRequest : class
    {
        var correlationLogManager = CorrelationUtil.GetCorrelationLogManagerFromContextHeaders(ctx);
        var flowSagaService = _serviceProvider.GetRequiredService<IFlowSagaService>();
        
        await flowSagaService.SendFlowRequestAsync(correlationLogManager, request);
    }
}

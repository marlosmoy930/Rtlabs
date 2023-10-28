
using ESD.Domain.Documents.Enums;
using FlowSagaContracts.Document;
using FlowSagaContracts.Notification;
using FlowSagaContracts.UserInteraction;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Messages;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Utils;
using RTLabs.EisUks.FlowEngine.MassTransit.Logging.Helpers;
using RTLabs.EisUks.FlowEngine.MassTransit.Settings.Configuration;

namespace FlowSagas.UserInteraction;

public class UserInteractionSaga : MassTransitStateMachine<UserInteractionSagaInstance>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserInteractionSaga> _logger;
    private readonly string _rabbitMqConnectionString;
    private static Event<FlowSagaStartCommand<UserInteractionSagaData>> StartSagaEvent { get; set; }
    private static Event<UserInteractionData> UserInteractionEvent { get; set; }
    private static Event<FlowSagaEndCommand> EndSagaEvent { get; set; }

    public State Launched { get; set; }

    public UserInteractionSaga(
        IServiceProvider serviceProvider,
        ILogger<UserInteractionSaga> logger,
        RabbitMqConnectionStringProvider rabbitMqConnectionStringProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _rabbitMqConnectionString = rabbitMqConnectionStringProvider.ConnectionString;
        InstanceState(x => x.CurrentState);
        Event(() => StartSagaEvent, x =>
        {
            x.CorrelateById(ctx => ctx.Message.CorrelationId);
            x.InsertOnInitial = true;
        });
        Event(() => UserInteractionEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => EndSagaEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Initially(
            When(StartSagaEvent)
                .ThenAsync(ProcessStartSagaEventAsync)
                .TransitionTo(Launched)
        );

        During(Launched,
            When(UserInteractionEvent)
                .ThenAsync(PerformUserInteractionActionAsync)
        );

        During(Launched,
            When(EndSagaEvent)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    private async Task ProcessStartSagaEventAsync(BehaviorContext<UserInteractionSagaInstance, FlowSagaStartCommand<UserInteractionSagaData>> ctx)
    {
        _logger.LogInformation("ProcessStartSagaEvent");
        
        var correlationLogManager = CorrelationUtil.GetCorrelationLogManagerFromContextHeaders(ctx);

        var instance = ctx.Saga;
        var command = ctx.Message;

        instance.UserId = command.UserId;
        instance.UserEmail = command.UserEmail;
        instance.QueuedAt = command.StartedAt;
        instance.LogCorrelationId = correlationLogManager.LogCorrelationId;
        instance.ParentSagaAddress = ctx.SourceAddress!.AbsoluteUri;
        instance.ParentSagaCorrelationId = command.ParentSagaCorrelationId!.Value;
        instance.ParentSagaFlowTaskId = command.TaskId!.Value;

        if (command.FlowData.NeedsRegistration)
        {
            await SendFlowRequestAsync(
                ctx,
                new ChangeDocumentRegistrationRequest
                {
                    DocumentRegistrationUserInteractionSagaId = command.CorrelationId,
                    DocumentId = command.FlowData.DocumentId,
                }
            );

            await SendFlowRequestAsync(
                ctx,
                new SetDocumentStatusRequest
                {
                    SagaId = command.CorrelationId,
                    DocumentId = command.FlowData.DocumentId,
                    DocumentStatus = DocumentStatus.OnRegistration
                }
            );

            if (command.UserId.HasValue)
            {
                await SendFlowRequestAsync(
                    ctx,
                    new NotificationNeedToRegisterRequest
                    {
                        DocumentId = command.FlowData.DocumentId,
                        DepartmentId = command.FlowData.DepartmentId,
                        StageId = command.FlowData.StageId,
                    });
            }
        }

        if (command.FlowData.NeedsAcquaintance)
        {
            await SendFlowRequestAsync(
                ctx,
                new ChangeDocumentAcquaintanceRequest
                {
                    DocumentAcquaintanceUserInteractionSagaId = command.CorrelationId,
                    DocumentId = command.FlowData.DocumentId,
                    StageId = command.FlowData.StageId,
                }
            );

            await SendFlowRequestAsync(
                ctx,
                new SetDocumentStatusRequest
                {
                    SagaId = command.CorrelationId,
                    DocumentId = command.FlowData.DocumentId,
                    DocumentStatus = DocumentStatus.OnAcquaintance
                }
            ); 
        }
    }

    private async Task PerformUserInteractionActionAsync(BehaviorContext<UserInteractionSagaInstance, UserInteractionData> ctx)
    {
        _logger.LogInformation("PerformUserInteractionActionAsync");
        await SendResponseToParentSagaAsync(ctx, ctx.Message.UserInteractionResult);
    }

    private async Task SendResponseToParentSagaAsync(SagaConsumeContext<UserInteractionSagaInstance> ctx, string result)
    {
        var instance = ctx.Saga;
        var response = GetParentSagaResponse(instance, new UserInteractionSagaData
        {
            EndData = result
        });

        var endpoint = await ctx.GetSendEndpoint(new Uri(instance.ParentSagaAddress!));

        var faultAddress = FlowQueueUtil.GetUnifiedFaultAddress(_rabbitMqConnectionString);
        await endpoint.Send(response, c => { c.FaultAddress = faultAddress; });
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

    private static FlowResponse GetParentSagaResponse(UserInteractionSagaInstance instance, UserInteractionSagaData data)
    {
        const int parentSagaChunkIndex = 0;
        var responseData = new ChildSagaFlowResponseData<UserInteractionSagaData>
        {
            CorrelationId = instance.CorrelationId,
            FlowData = data
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
}
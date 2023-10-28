using System.Diagnostics;

using ESD.Domain.Enums;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Tasks;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Testing;
using FlowSagaContracts.UserInteraction;

using Microsoft.Extensions.Logging;

using FlowSagaContracts.Approving.ApprovalChain;

namespace FlowSagas.Testing;

[TestFlowSaga]
public class TestFlowSaga : FlowSaga<TestFlowSagaData, ITestFlowSagaContext>
{
    private readonly ILogger<TestFlowSaga> _logger;

    public TestFlowSaga(
        ILogger<TestFlowSaga> logger,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override void ConfigureFlow()
    {
        StartApprovalSaga()
            .ThenIf(
                s => s.Data.ApprovalSagaResultType != ApprovalSagaStepResultType.Rejected,
                StartUserInteractionSaga(1)
            )
            .ThenIf(
                s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Rejected,
                Do(r1))
            ;
    }

    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> StartApprovalSaga()
    {
        return Create.ChildSaga<ApprovalSagaData, IApprovalSagaContext>("Approval Saga")
            .SetRequestFactory(s =>
            {
                var sagaData = ApprovalSagaData.Create(
                    GetApprovalSteps(),
                    1,
                    1,
                    Guid.NewGuid(),
                    new Dictionary<string, string>(),
                    new Dictionary<FlowSagaAdditionalDataKey, string>(),
                    0);

                return sagaData;
            })
            .SetChunkResponseProcessor((s, chunkIndex) =>
            {
                s.CurrentSagaSpace.Data.ApprovalSagaResultType = s.ChildSagaSpace.Data.ApprovalSagaResultType;
            })
            .Start();
    }

    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> StartUserInteractionSaga(int documentId)
    {
        return Create.ChildSaga<UserInteractionSagaData, IApprovalSagaContext>("User Interaction Saga")
            .SetRequestFactory(s =>
            {
                var sagaData = new UserInteractionSagaData()
                {
                    DocumentId = documentId,
                };
                return sagaData;
            })
            .SetChunkResponseProcessor((s, chunkIndex) =>
            {
                s.CurrentSagaSpace.Data.UserInteractionSagaResult = s.ChildSagaSpace.Data.EndData;
                _logger.LogInformation("User interaction result: {0}", s.ChildSagaSpace.Data.EndData);
            })
            .Start();
    }

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r1
        => SendDialog<TestFlowSagaRequest1>(500, 500);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r2
        => SendDialog<TestFlowSagaRequest2>(700, 700);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r3
        => SendDialog<TestFlowSagaRequest3>(700, 700);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r4
        => SendDialog<TestFlowSagaRequest4>(500, 500);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r5
        => SendDialog<TestFlowSagaRequest5>(1500, 500);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r6
        => SendDialog<TestFlowSagaRequest6>(500, 1500);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> r7
        => SendDialog<TestFlowSagaRequest7>(500, 500);

    [DebuggerHidden]
    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> rMissing1
        => SendDialog<TestFlowSagaRequestMissingConsumer1>(500, 500);

    private static List<ApprovalSagaStep> GetApprovalSteps()
    {
        var allowedUserIds = new HashSet<Guid>()
        {
            new("4221e0ef-e970-4230-a3ad-6a322d88a81b"),
            new("eb67a707-6ae6-4f31-a93a-4cd4d113c82f"),
            new("a83277e5-52fa-47f1-89ea-dbe6a85188a7"),
        };
        var step1 = new ApprovalSagaStep
        {
            Name = "Step1",
            ApprovalTaskAssignees = allowedUserIds.Take(1).Select(x => new ApprovalTaskAssigneeWithTemplate
            {
                Id = x
            }).ToList(),
        };
        var step2 = new ApprovalSagaStep
        {
            Name = "Step2",
            ApprovalTaskAssignees = allowedUserIds.Skip(1).Take(1).Select(x => new ApprovalTaskAssigneeWithTemplate
            {
                Id = x
            }).ToList(),
        };

        var steps = new List<ApprovalSagaStep>() { step1, step2 };
        return steps;
    }

    private FlowTaskChain<TestFlowSagaData, ITestFlowSagaContext> SendDialog<TRequest>(int requestDelay, int responseDelay)
        where TRequest : class, new()
    {
        var dialog = Create.Dialog<TRequest, TestFlowSagaResponse>();
        dialog.SetRequestFactory(async s =>
        {
            await Task.Delay(RandomUtil.GetRandom(requestDelay));
            return new TRequest();
        });
        dialog.SetCompleteProcessor(async s =>
        {
            await Task.Delay(RandomUtil.GetRandom(responseDelay));
        });
        dialog.Name = typeof(TRequest).Name;

        return Send(dialog);
    }
}

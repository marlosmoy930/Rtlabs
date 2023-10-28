using FlowSagaContracts.Approving;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services;

namespace FlowSagas.Testing;

public class TestFlowSagaData : ILaunchSettings
{
    public ApprovalSagaStepResultType? ApprovalSagaResultType { get; set; }
    public string UserInteractionSagaResult { get; set; }
}

public class TestCircuitFlowSagaData : ILaunchSettings
{
    public ApprovalSagaStepResultType? OpenApprovalSagaResultType { get; set; }
    public string OpenUserInteractionSagaResult { get; set; }
}

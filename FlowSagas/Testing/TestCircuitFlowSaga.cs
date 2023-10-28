using FlowSagaContracts.Testing;
using Microsoft.Extensions.Logging;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;
using Services.Interfaces;

namespace FlowSagas.Testing;

[TestFlowSaga]
public class TestCircuitFlowSaga : FlowSaga<TestCircuitFlowSagaData, ITestCircuitFlowSagaContext>
{
    private readonly ICircuitFlowEngineService _circuitFlowEngineService;

    public TestCircuitFlowSaga(
        IServiceProvider serviceProvider, 
        ICircuitFlowEngineService circuitFlowEngineService) : base(serviceProvider)
    {
        _circuitFlowEngineService = circuitFlowEngineService;
    }

    protected override void ConfigureFlow()
    {
        DoUnsafe(nameof(TestCircuitFlowSaga), async s =>
        {
            var data = new TestFlowSagaData();
            await _circuitFlowEngineService.StartSagaAsync(s.BehaviorContext, data, CancellationToken.None);
        });
    }
}
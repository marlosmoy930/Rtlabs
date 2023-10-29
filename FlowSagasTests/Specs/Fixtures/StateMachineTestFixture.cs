using MassTransit; 
using MassTransit.Testing; 
using Microsoft.Extensions.Logging; 
using Xunit; 

namespace FlowSagasTests.Specs.Fixtures;
  
public class StateMachineTestFixture<TStateMachine, TInstance> : IAsyncLifetime
    where TStateMachine : class, SagaStateMachine<TInstance>
    where TInstance : class, SagaStateMachineInstance
{ 
    private readonly TStateMachine _machine;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISagaStateMachineTestHarness<TStateMachine, TInstance> _sagaHarness;
    private readonly ITestHarness _testHarness;
    private readonly ILogger _output;

    public TStateMachine Machine => _machine; 
    public ISagaStateMachineTestHarness<TStateMachine, TInstance> SagaHarness => _sagaHarness;
    public ITestHarness TestHarness => _testHarness;  
    public IServiceProvider ServiceProvider => _serviceProvider;
    public ILogger Output => _output;

    public StateMachineTestFixture(   
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory, 

        ISagaStateMachineTestHarness<TStateMachine, TInstance> sagaHarness,
        TStateMachine machine,
        ITestHarness testHarness)
    {
        _output = loggerFactory.CreateLogger("Tests");
        _serviceProvider = serviceProvider;
        _sagaHarness = sagaHarness;
        _machine = machine;
        _testHarness = testHarness;
           
        LogContext.ConfigureCurrentLogContext(loggerFactory);
    }

    public async Task InitializeAsync()
    {  
        await _testHarness.Start();
    }

    public async Task DisposeAsync()
    {
        try
        {
            await _testHarness.Stop();

        }
        finally
        {
            await ((IAsyncDisposable)_serviceProvider).DisposeAsync();
        }
    } 
}
using ESD.Domain.Enums;
using FlowConsumers.ApprovalTasks;
using FlowSagaContracts.Holiday;
using FlowSagas.HolidayProcess;
using FlowSagasTests.Specs.Fixtures;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Instances;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Messages;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Services; 
using Xunit; 

namespace FlowSagasTests.Specs
{
    public class HolidayFlowSpecs : 
        IClassFixture<StateMachineTestFixture<HolidayFlowSaga, FlowInstance<HolidayFlowSagaData, IHolidayFlowSagaContext>>>
    {  
        private readonly ITestHarness _testHarness; 
        private readonly ISagaStateMachineTestHarness<HolidayFlowSaga, FlowInstance<HolidayFlowSagaData, IHolidayFlowSagaContext>> _sagaHarness;
        private readonly ILogger _output;
        private readonly HolidayFlowSaga _machine;
        private readonly IServiceProvider _provider;

        public HolidayFlowSpecs(StateMachineTestFixture<HolidayFlowSaga, FlowInstance<HolidayFlowSagaData, IHolidayFlowSagaContext>> fixture)
        {
            _testHarness = fixture.TestHarness; 
            _sagaHarness = fixture.SagaHarness;
            _machine = fixture.Machine;
            _output = fixture.Output;
            _provider = fixture.ServiceProvider;
        }
         
        [Fact]
        public async Task When_saga_starts_correlation_id_is_created_Async()
        {   
            using var scope = _provider.CreateScope();

            var svc = scope.ServiceProvider.GetRequiredService<ICorrelationFlowSagaService>();

            var holidayFlowSagaData = new HolidayFlowSagaData
            {
                StageCode = StaffProcessStageCode.HolidayStatement
            };

            var correlationId = await svc.StartSagaAsync(holidayFlowSagaData, (IHolidayFlowSagaContext _) => { });

            correlationId.Should().NotBeEmpty(); 
             
            //(await _testHarness.Consumed.Any<FlowSagaStartCommand<HolidayFlowSagaData>>()).Should()
            //    .BeTrue($"The {nameof(FlowSagaStartCommand<HolidayFlowSagaData>)} should be consumed by Masstransit!");
            //(await _sagaHarness.Consumed.Any<FlowSagaStartCommand<HolidayFlowSagaData>>()).Should()
            //    .BeTrue($"The {nameof(FlowSagaStartCommand<HolidayFlowSagaData>)} should be consumed by Saga!");
            //(await _sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should()
            //    .BeTrue("The correlationId of holiday flow should be match!");
            //_sagaHarness.Created.ContainsInState(correlationId, _machine, _machine.Launched).Should()
            //    .NotBeNull("Saga instance of holiday flow should be exists!");
            //(await _sagaHarness.Exists(correlationId, x => x.Launched)).HasValue.Should()
            //    .BeTrue("Saga instance should exists!");

            //_output.LogInformation("Ran in here");

            await _testHarness.OutputTimeline(Console.Out, options => options.Now().IncludeAddress());
        }
    }
}
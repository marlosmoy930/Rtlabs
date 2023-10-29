using FlowConsumers.ApprovalTasks;
using FlowSagaContracts.Holiday;
using FlowSagas.HolidayProcess;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RTLabs.EisUks.FlowEngine.Core.Configuration.LogManager;
using RTLabs.EisUks.FlowEngine.Core.Utils;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Instances;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Logging.SagaContext;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Logging.Sagas;
using RTLabs.EisUks.FlowEngine.FlowSagaEngine.Settings;
using RTLabs.EisUks.FlowEngine.MassTransit.Settings.Configuration;
using RTLabs.EisUks.FlowEngine.Redis; 
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Logging; 

namespace FlowSagasTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<HolidayFlowSaga, FlowInstance<HolidayFlowSagaData, IHolidayFlowSagaContext>>()
                        .InMemoryRepository();

                    cfg.AddConsumer<UpdateHolidayStatementStateRequestConsumer>();
                    cfg.AddConsumer<UpdateHolidayOrderStateRequestConsumer>();
                    cfg.AddConsumer<UpdateHolidayOrderRegistrationDataRequestConsumer>();
                });

            services.AddScoped<ICorrelationLogManager, CorrelationLogManager>();
            services.UseFlowSagaService<MockFlowSagaContextRepository>();
            services.AddTransient<IHttpContextAccessor>(_ => new Mock<IHttpContextAccessor>().Object);
            services.AddTransient<ISagaContextLogger>(_ => new Mock<ISagaContextLogger>().Object);
            services.AddTransient<IFlowSagaLogger>(_ => new Mock<IFlowSagaLogger>().Object);
            services.AddTransient<ILogger<HolidayFlowSagaData>>(_ => new Mock<ILogger<HolidayFlowSagaData>>().Object);
            services.AddSingleton(new RabbitMqConnectionStringProvider { ConnectionString = "loopback://localhost/" });
            services.AddSingleton(new FlowEngineSettingsStorage(new FlowEngineSettings()));
             
            services.SetLogger(); 
        }

        public void Configure(ILoggerFactory loggerFactory, ITestOutputHelperAccessor accessor) =>
            loggerFactory.AddProvider(new XunitTestOutputLoggerProvider(accessor));
    }
}

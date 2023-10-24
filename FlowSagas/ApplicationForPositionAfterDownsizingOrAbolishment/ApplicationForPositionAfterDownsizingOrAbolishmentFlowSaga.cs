using ESD.Domain.Documents.Enums;

using FlowSagaContracts.ApplicationForPositionAfterDownsizingOrAbolishment;
using FlowSagaContracts.Approving;

using FlowSagas.FlowChainBuilding;

using Microsoft.Extensions.Logging;

using RTLabs.EisUks.FlowEngine.FlowSagaEngine.FlowControl.Sagas;

namespace FlowSagas.ApplicationForPositionAfterDownsizingOrAbolishment
{
    public class ApplicationForPositionAfterDownsizingOrAbolishmentFlowSaga : FlowSaga<ApplForPostAfterDownsizingOrAbolishmentFlowSagaData,
        IApplicationForPositionAfterDownsizingOrAbolishmentFlowContext>
    {
        private readonly ILogger<ApplicationForPositionAfterDownsizingOrAbolishmentFlowSaga> _logger;

        public ApplicationForPositionAfterDownsizingOrAbolishmentFlowSaga(IServiceProvider serviceProvider,
            ILogger<ApplicationForPositionAfterDownsizingOrAbolishmentFlowSaga> logger) : base(serviceProvider)
        {
            _logger = logger;
        }

        protected override void ConfigureFlow()
        {
            Create.StartApprovalSagaWithNotification() //TODO - Notification?
                .ThenIf(
                    s => s.Data.ApprovalSagaResultType == ApprovalSagaStepResultType.Approved,
                    Do(Create.SendSetDocumentStatusRequest(DocumentStatus.Finished))
                );
        }
    }
}

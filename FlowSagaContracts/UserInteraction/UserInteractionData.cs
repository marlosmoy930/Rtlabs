using MassTransit;

namespace FlowSagaContracts.UserInteraction;

public class UserInteractionData: CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    
    public string UserInteractionResult { get; set; }
}
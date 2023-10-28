using MassTransit;

namespace FlowSagas.UserInteraction;

public class UserInteractionSagaInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public string? LogCorrelationId { get; set; }
    public DateTime QueuedAt { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public Guid ParentSagaCorrelationId { get; set; }
    public int ParentSagaFlowTaskId { get; set; }
    public string? ParentSagaAddress { get; set; }
}
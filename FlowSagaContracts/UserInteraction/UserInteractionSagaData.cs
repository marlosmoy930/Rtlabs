namespace FlowSagaContracts.UserInteraction;

public class UserInteractionSagaData
{
    public int DocumentId { get; set; }

    public Guid DepartmentId { get; set; }

    public string EndData { get; set; }

    public bool NeedsRegistration { get; set; } 

    public bool NeedsAcquaintance { get; set; }
    
    public int StageId { get; set; }
}

using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.StaffProcessEvent
{
    public class StaffProcessEventRequest
    {
        public Guid ApprovalSagaInstanceId { get; init; }

        public int DocumentId { get; init; }

        public DateTime DateTimeUtc { get; init; }
        
        public StaffProcessEventType StaffProcessEventType { get; init; }

        public Guid? UserId { get; init; }
    }
}

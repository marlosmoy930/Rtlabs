namespace FlowSagaContracts.StaffProcessSubscriber
{
    public class RemoveStaffProcessSubscriberRequest
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
    }
}

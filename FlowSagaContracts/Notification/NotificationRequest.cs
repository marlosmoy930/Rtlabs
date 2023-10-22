namespace FlowSagaContracts.Notification
{
    public abstract record NotificationRequest
    {
        public Guid UserId { get; init; }

        public int DocumentId { get; init; }
    }
}

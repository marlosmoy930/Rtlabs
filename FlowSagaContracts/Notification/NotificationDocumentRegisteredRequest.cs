namespace FlowSagaContracts.Notification
{
    public record NotificationDocumentRegisteredRequest
    {
        public Guid ApprovalSagaCorrelationId { get; set; }

        public int DocumentId { get; set; }
    }
}

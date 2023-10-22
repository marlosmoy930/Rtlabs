namespace FlowSagaContracts.Notification
{
    public record NotificationDocumentApprovedRequest
    {
        public Guid ApprovalSagaCorrelationId { get; set; }

        public int DocumentId { get; set; }
    }
}

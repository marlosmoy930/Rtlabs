
namespace FlowSagaContracts.Holiday;

public class UpdateHolidayOrderAcquaintanceDateRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }
}
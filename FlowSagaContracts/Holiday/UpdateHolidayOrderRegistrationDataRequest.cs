
namespace FlowSagaContracts.Holiday;

public class UpdateHolidayOrderRegistrationDataRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }
}
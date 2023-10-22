using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.Holiday;

public class UpdateHolidayStatementStateRequest
{
    public Guid ApprovalSagaCorrelationId { get; set; }

    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}
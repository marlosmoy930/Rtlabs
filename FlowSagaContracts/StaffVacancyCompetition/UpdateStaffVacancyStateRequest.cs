using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.StaffVacancyCompetition;

public class UpdateStaffVacancyStateRequest
{
    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}

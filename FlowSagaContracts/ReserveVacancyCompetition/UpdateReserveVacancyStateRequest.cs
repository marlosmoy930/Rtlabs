using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.ReserveVacancyCompetition;

public class UpdateReserveVacancyStateRequest
{
    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}

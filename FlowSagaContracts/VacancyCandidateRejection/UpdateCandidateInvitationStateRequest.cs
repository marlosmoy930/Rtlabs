using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.VacancyCandidateRejection;

public class UpdateCandidateRejectionStateRequest
{
    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}

using ESD.Domain.StaffProcesses.Enums;

namespace FlowSagaContracts.VacancyCandidateInvitation;

public class UpdateCandidateInvitationStateRequest
{
    public int DocumentId { get; set; }

    public StaffProcessStageState State { get; set; }
}

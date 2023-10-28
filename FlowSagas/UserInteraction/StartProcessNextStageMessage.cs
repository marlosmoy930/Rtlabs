using Services.Models.StaffProcess;

namespace FlowSagas.UserInteraction;

public record StartProcessNextStageMessage
{
    public ApprovalChainTemplateDto NextStageApprovalChainTemplate { get; set; }
    public int NextStageDocumentId { get; set; }
}
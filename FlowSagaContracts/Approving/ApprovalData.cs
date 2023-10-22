using ESD.Domain.Enums;

using FlowSagaContracts.Approving.ApprovalChain;

namespace FlowSagaContracts.Approving;

public class ApprovalData
{
    public ApprovalSagaChain ApprovalChain { get; set; }

    public Dictionary<string, string> TaskNameTemplateSubstitutions { get; set; }

    public Guid DepartmentId { get; set; }

    public Dictionary<FlowSagaAdditionalDataKey, string> AdditionalData { get; set; }

    public Guid? SubjectId
    {
        get
        {
            if (AdditionalData == null)
            {
                return null;
            }

            if (AdditionalData.TryGetValue(FlowSagaAdditionalDataKey.SubjectId, out var subjectId))
            {
                return Guid.Parse(subjectId);
            }

            return null;
        }
    }

    public Guid? CommissionMeetingId
    {
        get
        {
            if (AdditionalData == null)
            {
                return null;
            }

            if (AdditionalData.TryGetValue(FlowSagaAdditionalDataKey.CommissionMeetingId, out var commissionMeetingId))
            {
                return Guid.Parse(commissionMeetingId);
            }

            return null;
        }
    }

    public StaffProcessStageCode StageCode { get; set; }
}

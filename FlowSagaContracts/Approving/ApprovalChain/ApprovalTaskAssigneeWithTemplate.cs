using ESD.Domain.StaffProcesses.Entities;

namespace FlowSagaContracts.Approving.ApprovalChain;

public class ApprovalTaskAssigneeWithTemplate : ApprovalTaskAssigneeWithApprovalAction
{
    /// <summary>
    /// Шаблон исполнителя задачи (из шаблона маршрута согласования КП).
    /// </summary>
    public AssigneeTemplate? AssigneeTemplate { get; set; }
}

public static class ApprovalSagaAssigneeWithTemplateConverter
{
    public static ApprovalTaskAssignee ToApprovalTaskAssignee(this ApprovalTaskAssigneeWithTemplate approvalSagaAssigneeWithTemplate)
    {
        return new ApprovalTaskAssignee
        {
            Id = approvalSagaAssigneeWithTemplate.Id,
            Name = approvalSagaAssigneeWithTemplate.Name,
            Position = approvalSagaAssigneeWithTemplate.Position,
            StampFormat = approvalSagaAssigneeWithTemplate.StampFormat,
            SignProcessStartType = approvalSagaAssigneeWithTemplate.SignProcessStartType
        };
    }
}


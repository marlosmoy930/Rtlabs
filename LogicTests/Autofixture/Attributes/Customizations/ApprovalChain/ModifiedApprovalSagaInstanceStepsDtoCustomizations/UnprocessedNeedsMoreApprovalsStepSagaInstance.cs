using ApplicationLogic.ApprovalChainTemplates;
using ApplicationLogic.ApprovalChainModification.Queries.Validation;

using AutoFixture;

using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Approving.ApprovalChain;

using LogicTests.Autofixture.Constants;

namespace LogicTests.Autofixture.Attributes.Customizations.ModifiedApprovalSagaInstanceStepsDtoCustomizations;

public class UnprocessedNeedsMoreApprovalsStepSagaInstance : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ModifiedApprovalSagaInstanceDto>(f => f
            .With(s => s.UserId, TestUserIds.UserOne)
            .With(s => s.Steps, new[]
            {
                fixture.Build<ApprovalStepDto>()
                    .With(q => q.DaysToApprove, 10)
                    .With(q => q.ApprovalType, ApprovalType.Approve)
                    .With(q => q.Result, ApprovalSagaStepResultType.Approved)
                    .With(
                        s => s.Assignees, new []
                        {
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserOne)
                                .Create(),
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserTwo)
                                .Create()
                        }
                    )
                    .Create(),
                fixture.Build<ApprovalStepDto>()
                    .With(q => q.DaysToApprove, 10)
                    .With(q => q.ApprovalType, ApprovalType.Approve)
                    .With(q => q.Result, ApprovalSagaStepResultType.NeedsMoreApprovals)
                    .With(
                        s => s.Assignees, new []
                        {
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserTwo)
                                .Create(),
                        }
                    )
                    .Create(),

            })
        );
    }
}
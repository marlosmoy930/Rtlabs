using ApplicationLogic.ApprovalChainTemplates;
using ApplicationLogic.ApprovalChainModification.Queries.Validation;

using AutoFixture;

using FlowSagaContracts.Approving;

using LogicTests.Autofixture.Constants;

using FlowSagaContracts.Approving.ApprovalChain;

namespace LogicTests.Autofixture.Attributes.Customizations.ModifiedApprovalSagaInstanceStepsDtoCustomizations;

public class AllStepsHaveApprovedResultAndUserOneAssigneeSagaInstance : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ModifiedApprovalSagaInstanceDto>(f => f
            .With(s => s.UserId, TestUserIds.UserOne));

        fixture.Customize<ApprovalStepDto>(f => f
            .With(q => q.Result, ApprovalSagaStepResultType.Approved)
            .With(s => s.Assignees, new[]
                {
                    fixture
                        .Build<ApprovalTaskAssigneeWithTemplate>()
                        .With(x => x.Id, TestUserIds.UserOne)
                        .Create(),
                }
            ));
    }
}
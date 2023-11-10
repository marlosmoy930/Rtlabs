using ApplicationLogic.ApprovalChainTemplates;
using ApplicationLogic.ApprovalChainModification.Queries.Validation;

using AutoFixture;

using ESD.Domain.StaffProcesses.Enums;

using FlowSagaContracts.Approving;
using FlowSagaContracts.Approving.ApprovalChain;

using LogicTests.Autofixture.Constants;

using ESD.Domain.StaffProcesses.Entities;

namespace LogicTests.Autofixture.Attributes.Customizations.ApprovalChain.ModifiedApprovalSagaInstanceStepsDtoCustomizations;

internal class ApprovedAndAwaitingAndUnprocessedStepsSagaInstance : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ModifiedApprovalSagaInstanceDto>(f => f
            .With(s => s.UserId, TestUserIds.UserOne)
            .With(s => s.CurrentStepIndex, 1)
            .With(s => s.Steps, new[]
            {
                fixture.Build<ApprovalStepDto>()
                    .With(q => q.Name, TestStepNames.StepZero)
                    .With(q => q.DaysToApprove, 10)
                    .With(q => q.ApprovalType, ApprovalType.Approve)
                    .With(q => q.Result, ApprovalSagaStepResultType.Approved)
                    .With(
                        s => s.Assignees, new []
                        {
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserOne)
                                .With(x => x.AssigneeTemplate, new AssigneeTemplate
                                                                {
                                                                    AssigneeId = TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeId,
                                                                    Type = TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeType
                                                                }
                                    )
                                .Create(),
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserTwo)
                                .With(x => x.AssigneeTemplate, new AssigneeTemplate
                                                                {
                                                                    AssigneeId = TestAssigneeTemplates.AssigneeTemplateForUserTwoAssigneeId,
                                                                    Type = TestAssigneeTemplates.AssigneeTemplateForUserTwoAssigneeType
                                                                }
                                    )
                                .Create()
                        }
                    )
                    .Create(),
                fixture.Build<ApprovalStepDto>()
                    .With(q => q.Name, TestStepNames.StepOne)
                    .With(q => q.DaysToApprove, 10)
                    .With(q => q.ApprovalType, ApprovalType.Approve)
                    .With(q => q.Result, ApprovalSagaStepResultType.Awaiting)
                    .With(
                        s => s.Assignees, new []
                        {
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserOne)
                                .With(x => x.AssigneeTemplate, new AssigneeTemplate
                                                                {
                                                                    AssigneeId = TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeId,
                                                                    Type = TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeType
                                                                }
                                    )
                                .Create(),
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserTwo)
                                .Create()
                        }
                    )
                    .Create(),
                fixture.Build<ApprovalStepDto>()
                    .With(q => q.Name, TestStepNames.StepTwo)
                    .With(q => q.DaysToApprove, 10)
                    .With(q => q.ApprovalType, ApprovalType.Approve)
                    .With(q => q.Result, (ApprovalSagaStepResultType?)null)
                    .With(
                        s => s.Assignees, new []
                        {
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserOne)
                                .With(x => x.AssigneeTemplate, new AssigneeTemplate
                                                                {
                                                                    AssigneeId = TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeId,
                                                                    Type = TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeType
                                                                }
                                )
                                .Create(),
                            fixture
                                .Build<ApprovalTaskAssigneeWithTemplate>()
                                .With(x => x.Id, TestUserIds.UserTwo)
                                .With(x => x.AssigneeTemplate, new AssigneeTemplate
                                                                {
                                                                    AssigneeId = TestAssigneeTemplates.AssigneeTemplateForUserTwoAssigneeId,
                                                                    Type = TestAssigneeTemplates.AssigneeTemplateForUserTwoAssigneeType
                                                                }
                                )
                                .Create()
                        }
                    )
                    .Create()
                }
            )
        );
    }
}
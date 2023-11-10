using ApplicationLogic.ApprovalChainModification.Commands;
using ApplicationLogic.ApprovalChainModification.Queries.Validation;

using AutoFixture;

using ESD.Domain.StaffProcesses.Entities;
using ESD.Domain.StaffProcesses.Enums;

using LogicTests.Autofixture.Constants;

namespace LogicTests.Autofixture.Attributes.Customizations.ApprovalChain.VerifyIsValidModifiedChainQueryCustomizations;

public class AssigneeTemplateDeletedQuery : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<VerifyIsValidModifiedChainQuery>(f => f
            .With(s => s.UserId, TestUserIds.UserOne)
            .With(s => s.ModifiedChain,
                fixture
                    .Build<ModifiedChainStepsDto>()
                    .With(
                        s => s.Steps, new List<ApprovalStep>
                        {
                        fixture.Build<ApprovalStep>()
                            .With(q => q.Name, TestStepNames.StepTwo)
                            .With(q => q.DaysToApprove, 10)
                            .With(q => q.ApprovalType, ApprovalType.Approve)
                            .With(
                                s => s.AssigneeTemplates, new []
                                {
                                    fixture
                                        .Build<AssigneeTemplate>()
                                        .With(x => x.AssigneeId, TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeId)
                                        .With(x => x.Type, TestAssigneeTemplates.AssigneeTemplateForUserOneAssigneeType)
                                        .Create()
                                }
                            )
                            .Create()
                        }
                    )
                    .Create())
        );
    }
}
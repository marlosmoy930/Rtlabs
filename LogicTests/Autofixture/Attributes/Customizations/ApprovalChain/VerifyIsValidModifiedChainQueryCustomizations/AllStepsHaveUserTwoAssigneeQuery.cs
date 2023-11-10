using ApplicationLogic.ApprovalChainModification.Queries.Validation;

using AutoFixture;

using ESD.Domain.StaffProcesses.Entities;

using LogicTests.Autofixture.Constants;

namespace LogicTests.Autofixture.Attributes;

public class AllStepsHaveUserTwoAssigneeQuery : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<VerifyIsValidModifiedChainQuery>(f => f
            .With(q => q.UserId, TestUserIds.UserOne));
             
        fixture.Customize<ApprovalStep>(f => f
            .With(s => s.AssigneeTemplates, new[]
                {
                    fixture
                        .Build<AssigneeTemplate>()
                        .With(x => x.AssigneeId, TestUserIds.UserTwo)
                        .Create(),
                }
            ));
    }
}
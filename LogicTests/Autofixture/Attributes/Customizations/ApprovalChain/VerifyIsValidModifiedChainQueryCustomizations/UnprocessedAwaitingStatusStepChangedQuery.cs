using ApplicationLogic.ApprovalChainModification.Commands;
using ApplicationLogic.ApprovalChainModification.Queries.Validation;

using AutoFixture;

using ESD.Domain.StaffProcesses.Entities;
using ESD.Domain.StaffProcesses.Enums;

using LogicTests.Autofixture.Constants;

namespace LogicTests.Autofixture.Attributes.Customizations.VerifyIsValidModifiedChainQueryCustomizations;

public class UnprocessedAwaitingStatusStepChangedQuery : ICustomization
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
                            fixture
                                .Build<ApprovalStep>()
                                .With(q => q.Name, TestStepNames.StepOne)
                                .With(q => q.DaysToApprove, 10)
                                .With(q => q.ApprovalType, ApprovalType.Approve)
                                .With(
                                    s => s.AssigneeTemplates, new []
                                    {
                                        fixture
                                            .Build<AssigneeTemplate>()
                                            .With(x => x.AssigneeId, TestUserIds.UserOne)
                                            .Create(),
                                        fixture
                                            .Build<AssigneeTemplate>()
                                            .With(x => x.AssigneeId, TestUserIds.UserTwo)
                                            .Create()
                                    }
                                )
                                .Create(),
                            fixture.Build<ApprovalStep>()
                                .With(q => q.Name, TestStepNames.StepTwo)
                                .With(q => q.DaysToApprove, 10)
                                .With(q => q.ApprovalType, ApprovalType.Approve)
                                .With(
                                    s => s.AssigneeTemplates, new []
                                    {
                                        fixture
                                            .Build<AssigneeTemplate>()
                                            .With(x => x.AssigneeId, TestUserIds.UserTwo)
                                            .Create(),
                                        fixture
                                            .Build<AssigneeTemplate>()
                                            .With(x => x.AssigneeId, TestUserIds.UserThree)
                                            .Create()
                                    }
                                )
                                .Create(),

                        })
                    .Create())
        );
    }
}
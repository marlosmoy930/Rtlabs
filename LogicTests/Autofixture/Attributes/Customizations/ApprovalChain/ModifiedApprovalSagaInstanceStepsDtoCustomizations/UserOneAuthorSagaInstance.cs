using ApplicationLogic.ApprovalChainModification.Queries.Validation;
using AutoFixture;
using LogicTests.Autofixture.Constants;

namespace LogicTests.Autofixture.Attributes.Customizations.ModifiedApprovalSagaInstanceStepsDtoCustomizations;

public class UserOneAuthorSagaInstance : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ModifiedApprovalSagaInstanceDto>(f => f
            .With(q => q.UserId, TestUserIds.UserOne));
    }
}
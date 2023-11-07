using AutoFixture;
using AutoFixture.Xunit2;
using LogicTests.Autofixture.Attributes.Customizations.ApprovalChain;
using LogicTests.Autofixture.Attributes.Customizations.ModifiedApprovalSagaInstanceStepsDtoCustomizations;
using LogicTests.Autofixture.Attributes.Customizations.VerifyIsValidModifiedChainQueryCustomizations;

namespace LogicTests.Autofixture.Attributes.Attributes.AprovalChain
{
    public class ChainCreationModificationDifferentAuthorsAutoDataAttribute : AutoDataAttribute
    {
        public ChainCreationModificationDifferentAuthorsAutoDataAttribute()
            : base(() => new Fixture().Customize(
                new CompositeCustomization(
                    new ApprovalStepModificationVerifierCustomization(),
                    new UserOneAuthorSagaInstance(),
                    new UserTwoAuthorChainModificationQuery())))
        {
        }
    }
}
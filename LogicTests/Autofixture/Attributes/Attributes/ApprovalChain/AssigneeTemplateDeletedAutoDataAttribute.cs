using AutoFixture;
using AutoFixture.Xunit2;

using LogicTests.Autofixture.Attributes.Customizations.ApprovalChain;
using LogicTests.Autofixture.Attributes.Customizations.ApprovalChain.VerifyIsValidModifiedChainQueryCustomizations;
using LogicTests.Autofixture.Attributes.Customizations.ModifiedApprovalSagaInstanceStepsDtoCustomizations;

namespace LogicTests.Autofixture.Attributes.Attributes.ApprovalChain
{
    public class AssigneeTemplateDeletedAutoDataAttribute : AutoDataAttribute
    {
        public AssigneeTemplateDeletedAutoDataAttribute()
            : base(() => new Fixture().Customize(
                new CompositeCustomization(
                    new ApprovalStepModificationVerifierCustomization(),
                    new OneAwaitingStepAndOneUnprocessedStepSagaInstance(),
                    new AssigneeTemplateDeletedQuery())))
        {
        }
    }
}

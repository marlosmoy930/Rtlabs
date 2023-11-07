using AutoFixture;
using AutoFixture.Xunit2;

using LogicTests.Autofixture.Attributes.Customizations.ApprovalChain;
using LogicTests.Autofixture.Attributes.Customizations.ApprovalChain.ModifiedApprovalSagaInstanceStepsDtoCustomizations;
using LogicTests.Autofixture.Attributes.Customizations.ApprovalChain.VerifyIsValidModifiedChainQueryCustomizations;
using LogicTests.Autofixture.Attributes.Customizations.ModifiedApprovalSagaInstanceStepsDtoCustomizations;

namespace LogicTests.Autofixture.Attributes.Attributes.ApprovalChain
{
    public class SagaWithoutResolvedStepsAssigneeTemplateAddedAutoDataAttribute : AutoDataAttribute
    {
        public SagaWithoutResolvedStepsAssigneeTemplateAddedAutoDataAttribute()
            : base(() => new Fixture().Customize(
                new CompositeCustomization(
                    new ApprovalStepModificationVerifierCustomization(),
                    new OneAwaitingStepAndOneUnprocessedStepSagaInstance(),
                    new AssigneeTemplateAddedForExistingStepQuery())))
        {
        }
    }

    public class SagaWithResolvedStepAssigneeTemplateAddedAutoDataAttribute : AutoDataAttribute
    {
        public SagaWithResolvedStepAssigneeTemplateAddedAutoDataAttribute()
            : base(() => new Fixture().Customize(
                new CompositeCustomization(
                    new ApprovalStepModificationVerifierCustomization(),
                    new ApprovedAndAwaitingAndUnprocessedStepsSagaInstance(),
                    new AssigneeTemplateAddedForExistingStepQuery())))
        {
        }
    }
}

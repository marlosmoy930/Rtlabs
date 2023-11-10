using ApplicationLogic.Services.ChainModification;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Community.AutoMapper;
using AutoFixture.Kernel;

namespace LogicTests.Autofixture.Attributes.Customizations.ApprovalChain;

public class ApprovalStepModificationVerifierCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(
            new TypeRelay(
                typeof(IApprovalStepModificationVerifier),
                typeof(ApprovalStepModificationVerifier)));

        fixture.Customize(new AutoMoqCustomization());

        fixture.Customize(new AutoMapperCustomization(x => x.AddMaps(typeof(ApplicationLogic.AutoMapperProfile))));
        fixture.Customize(new AutoMapperCustomization(x => x.AddMaps(typeof(Services.AutoMapperProfile))));
    }
}

using AutoFixture;
using AutoFixture.Xunit2;
using LogicTests.Autofixture.Attributes.Customizations.XmlStructureVerifier;
using Tests.Common.Autofixture.Customizations;

namespace LogicTests.Autofixture.Attributes.Attributes.XmlStructureVerifier;

public class VerifierAutoDataAttribute : AutoDataAttribute
{
    public VerifierAutoDataAttribute(bool createValidStructureXml)
        : base(() => new Fixture().Customize(
            new CompositeCustomization(
                new XmlStringFormatSerializerCustomization(),
                new VerifierCustomization(createValidStructureXml))))
    {
    }
}
using AutoFixture;
using AutoFixture.Xunit2;
using LogicTests.Autofixture.Attributes.Customizations.CanFillInTemplate;
using Tests.Common.Autofixture.Customizations; 

namespace LogicTests.Autofixture.Attributes.Attributes.CanFillInTemplate;

public class CanFillInTemplateAutoDataAttribute : AutoDataAttribute
{
    public CanFillInTemplateAutoDataAttribute(bool withRequiredTags, TagsPresence tagsPresence)
        : base(() => new Fixture().Customize(
            new CompositeCustomization( 
                new XmlStringFormatSerializerCustomization(),
                new CanFillInTemplateCustomization(withRequiredTags, tagsPresence))))
    {
    }
}
namespace Infrastructure.Contracts.Dto
{
    public record DocumentAttachmentMetadataDto
    {
        public string TemplateName { get; init; }

        public string TemplateCheckSum { get; init; }
    }
}

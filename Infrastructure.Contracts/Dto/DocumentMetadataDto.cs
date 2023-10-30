namespace Infrastructure.Contracts.Dto
{
    public record DocumentMetadataDto
    {
        public string TemplateName { get; init; }

        public string TemplateCheckSum { get; init; }
    }
}

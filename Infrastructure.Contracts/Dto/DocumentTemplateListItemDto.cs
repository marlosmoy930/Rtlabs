namespace Infrastructure.Contracts.Dto
{
    public class DocumentTemplateListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Типовой
        /// </summary>
        public bool IsDefault { get; set; }

        public string PositionName { get; set; }

        public int[] DocumentAttachmentTemplateIds { get; set; }
    }
}

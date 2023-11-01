namespace Infrastructure.Contracts.Dto
{
    public record FileDto
    {
        //Идентификатор из таблицы Documents или DocumentAttachments
        public int Id { get; init; }

        public bool IsAttachment { get; init; }

        public byte[] Content { get; init; }

        public string Name { get; init; }
    }
}

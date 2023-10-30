using System.IO;

namespace Infrastructure.Contracts.Dto;

public record CommonFileDto(string Name, Stream Content, string ContentType);

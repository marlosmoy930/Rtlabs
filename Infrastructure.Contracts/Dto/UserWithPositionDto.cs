using ESD.Domain.Documents.Enums;
using ESD.Domain.Enums;

namespace Infrastructure.Contracts.Dto
{
    public class UserWithPositionDto : NamedDto
    {
        public string Position { get; set; }

        public SignProcessStartType SignProcessStartType { get; set; }
        
        public SignedUserType? StampFormat { get; set; }
    }
}

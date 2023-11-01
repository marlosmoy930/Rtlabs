using System.Collections.Generic;

namespace Infrastructure.Contracts.Dto
{
    public class PagedResultOfNamedDto
    {
        public ICollection<NamedDto> Items { get; set; }

        public int Total { get; set; }
    }
}

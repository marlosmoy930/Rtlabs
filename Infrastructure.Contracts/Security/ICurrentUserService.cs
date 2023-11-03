using System;

namespace Infrastructure.Contracts.Security
{
    public interface ICurrentUserService
    {
        string SystemLogin { get; }

        Guid? GetCurrentUserId();
    }
}
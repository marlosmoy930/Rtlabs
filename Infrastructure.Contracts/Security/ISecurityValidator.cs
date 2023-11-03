using Infrastructure.Contracts.Security.Models;

namespace Infrastructure.Contracts.Security
{
    public interface ISecurityValidator<in TRequest>
    {
        bool Validate(TRequest request, SecurityContextInfo securityContext);
    }
}
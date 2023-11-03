using System.Threading.Tasks;
using Infrastructure.Contracts.Security.Models;

namespace Infrastructure.Contracts.Security
{
    public interface ISecurityContextProvider
    {
        Task<SecurityContextInfo> GetCurrentContextAsync();
    }
}
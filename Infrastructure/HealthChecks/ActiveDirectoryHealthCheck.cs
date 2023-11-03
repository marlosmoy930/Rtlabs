// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Diagnostics.HealthChecks;
//
// namespace Infrastructure.HealthChecks
// {
//     public class ActiveDirectoryHealthCheck : IHealthCheck
//     {
//         private readonly ICollection<SupportedDomain> _supportedDomains;
//
//         public ActiveDirectoryHealthCheck(IConfiguration configuration)
//         {
//             _supportedDomains = configuration.GetSection(SupportedDomain.AppSettingsKey).Get<ICollection<SupportedDomain>>();
//         }
//
//         public Task<HealthCheckResult> CheckHealthAsync(
//             HealthCheckContext context,
//             CancellationToken cancellationToken = default)
//         {
//             foreach (var supportedDomain in _supportedDomains)
//             {
//                 var domain = supportedDomain.Value.CleanModel();
//                 try
//                 {
//                     using var domainContext = new PrincipalContext(
//                         ContextType.Domain,
//                         domain.DomainName,
//                         domain.UserName,
//                         domain.Password);
//                 }
//                 catch (Exception ex)
//                 {
//                     return Task.FromResult(new HealthCheckResult(
//                         context.Registration.FailureStatus, $"LDAP query to '{supportedDomain.Key}' failed", ex));
//                 }
//             }
//
//             return Task.FromResult(HealthCheckResult.Healthy($"LDAP query to '{string.Join(",", _supportedDomains.Select(d => d.Key))}' successed"));
//         }
//     }
// }

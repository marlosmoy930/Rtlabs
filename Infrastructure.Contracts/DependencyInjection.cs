using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Contracts
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureContracts(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddAutoMapper(assembly);

            return services;
        }
    }
}
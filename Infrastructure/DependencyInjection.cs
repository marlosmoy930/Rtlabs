using System.Reflection;

using Infrastructure.Contracts.DateTime;
using Infrastructure.DateTime;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // services.AddScoped<ISecurityContextProvider, SecurityContextProvider>();
            // services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            // services.AddTransient<IUserSearcher, UserSearcher.UserSearcher>();
            // services.AddTransient<IFileUploadService, FileService.FileService>();
            // services.AddTransient<IFileDownloadService, FileService.FileService>();
            // services.AddTransient<ISendNotificationMessageService, SendNotificationMessageService>();
            services.AddMediatR(assembly);
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestSecurityValidationBehavior<,>));
            return services;
        }
    }
}

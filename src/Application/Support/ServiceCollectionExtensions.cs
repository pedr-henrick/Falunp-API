using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Support
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}

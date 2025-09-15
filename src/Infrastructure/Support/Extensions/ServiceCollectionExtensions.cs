using Domain.Interfaces;
using Infrastructure.Commons;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Support.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Support.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<ITokenService, TokenService>();

            // Commons
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddDbContext<InfrastructureDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.Configure<TokenOptions>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}

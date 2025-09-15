using Application.DTOs.Class;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Support
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IStudentService, StudentService>();

            // Validators
            services.AddScoped<IValidator<ClassDto>, ClassDtoValidator>();

            return services;
        }
    }
}

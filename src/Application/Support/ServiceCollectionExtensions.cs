using Application.DTOs.Class;
using Application.DTOs.Enrollment;
using Application.DTOs.Student;
using Application.Mappings;
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
            services.AddScoped<IEnrollmentService, EnrollmentService>();

            // Validators
            services.AddScoped<IValidator<ClassDto>, ClassDtoValidator>();
            services.AddScoped<IValidator<StudentCreateDto>, StudentCreateDtoValidator>();
            services.AddScoped<IValidator<StudentUpdateDto>, StudentUpdateDtoValidator>();
            services.AddScoped<IValidator<EnrollmentCreateDto>, EnrollmentCreateDtoValidator>();
            services.AddScoped<IValidator<EnrollmentUpdateDto>, PatchEnrollmentDtoValidator>();

            // Mappings
            MapsterConfig.RegisterMappings();

            return services;
        }
    }
}

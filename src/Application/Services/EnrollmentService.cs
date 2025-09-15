using Application.Common;
using Application.DTOs.Enrollment;
using Application.DTOs.Student;
using Domain.Entities;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class EnrollmentService(IEnrollmentRepository enrollmentRepository) : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;

        public async Task<Result<List<EnrollmentInfoDto>>> GetAsync(EnrollmentInfoDto enrollmentRequestDto, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetAsync(cancellationToken);
            var enrollmentDtos = enrollments.Select(e => new EnrollmentInfoDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student?.Name,
                ClassId = e.ClassId,
                ClassName = e.Class?.Name,
                RegistrationDate = e.RegistrationDate
            }).ToList();

            return Result<List<EnrollmentInfoDto>>.Success(enrollmentDtos);
        }

        public async Task<Result<string>> CreateAsync(EnrollmentCreateDto enrollmentCreatetDto, CancellationToken cancellationToken)
        {
            try
            {
                var enrollmentEntity = enrollmentCreatetDto.Adapt<Enrollment>();
                await _enrollmentRepository.CreateAsync(enrollmentEntity, cancellationToken);
                
                return Result<string>.Success("Enrollment added successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error retrieving enrollment: {ex.Message}");
            }
        }
    }
}

using Application.Common;
using Application.DTOs.Enrollment;
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
                StudentId = e.StudentId.Value,
                StudentName = e.Student?.Name,
                ClassId = e.ClassId.Value,
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

        public async Task<Result<string>> DeleteAsync(EnrollmentFilterDto enrollmentFilterDto, CancellationToken cancellationToken)
        {
            try
            {
                var enrolmentEntity = enrollmentFilterDto.Adapt<Enrollment>();
                await _enrollmentRepository.DeleteFilteredAsync(enrolmentEntity, cancellationToken);

                return Result<string>.Success("Enrollment successfully deleted");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error deleting enrollment: {ex.Message}");
            }
        }
    }
}

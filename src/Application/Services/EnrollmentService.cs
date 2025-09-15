using Application.Common;
using Application.DTOs.Enrollment;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class EnrollmentService(
        ILogger<EnrollmentService> logger,
        IEnrollmentRepository enrollmentRepository,
        IValidator<EnrollmentCreateDto> createValidator,
        IValidator<EnrollmentUpdateDto> updateValidator) : IEnrollmentService
    {
        private readonly ILogger<EnrollmentService> _logger = logger;
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
        private readonly IValidator<EnrollmentCreateDto> _createValidator = createValidator;
        private readonly IValidator<EnrollmentUpdateDto> _updateValidator = updateValidator;

        public async Task<Result<List<EnrollmentInfoDto>>> GetAsync(EnrollmentInfoDto enrollmentRequestDto, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentService - GetAsync] - Error retrieving enrollment");
                return Result<List<EnrollmentInfoDto>>.Failure($"Error retrieving enrollment: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateAsync(EnrollmentCreateDto enrollmentCreatetDto, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(enrollmentCreatetDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                        .ToList();
                    return Result<string>.ValidationFailure(errors);
                }

                var enrollmentEntity = enrollmentCreatetDto.Adapt<Enrollment>();
                await _enrollmentRepository.CreateAsync(enrollmentEntity, cancellationToken);

                return Result<string>.Success("Enrollment added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentService - CreateAsync] - Error retrieving enrollment");
                return Result<string>.Failure($"Error retrieving enrollment: {ex.Message}");
            }
        }

        public async Task<Result<string>> UpdateAsync(EnrollmentUpdateDto enrollmentUpdateDto, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(enrollmentUpdateDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                        .ToList();
                    return Result<string>.ValidationFailure(errors);
                }

                var enrollmentEntity = enrollmentUpdateDto.Adapt<Enrollment>();
                await _enrollmentRepository.UpdateAsync(enrollmentEntity, cancellationToken);

                return Result<string>.Success("Enrollment added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentService - UpdateAsync] - Error retrieving enrollment");
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
                _logger.LogError(ex, "[EnrollmentService - DeleteAsync] - Error deleting enrollment");
                return Result<string>.Failure($"Error deleting enrollment: {ex.Message}");
            }
        }
    }
}

using Application.Common;
using Application.DTOs.Student;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class StudentService(
        ILogger<StudentService> logger,
        IStudentRepository studentRepository,
        IValidator<StudentCreateDto> createValidator,
        IValidator<StudentUpdateDto> updateValidator,
        IPasswordHasher passwordHasher) : IStudentService
    {
        private readonly ILogger<StudentService> _logger = logger;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IValidator<StudentCreateDto> _createValidator = createValidator;
        private readonly IValidator<StudentUpdateDto> _updateValidator = updateValidator;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<List<StudentInfoDto>>> GetAsync(StudentFilterDto studentRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                var request = studentRequestDto.Adapt<Student>();
                var students = await _studentRepository.GetPagedAsync(request, studentRequestDto.PageNumber, studentRequestDto.PageSize, cancellationToken);

                var response = students.Adapt<List<StudentInfoDto>>();
                return Result<List<StudentInfoDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudentService - GetAsync] - Error retrieving students");
                return Result<List<StudentInfoDto>>.Failure($"Error retrieving students: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateAsync(StudentCreateDto studentCreateDto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(studentCreateDto, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                    .ToList();
                return Result<string>.ValidationFailure(errors);
            }

            try
            {
                var studentEntity = studentCreateDto.Adapt<Student>();
                studentEntity.SetPasswordHash(_passwordHasher.HashPassword(studentCreateDto.Password));

                await _studentRepository.CreateAsync(studentEntity, cancellationToken);
                return Result<string>.Success("Student added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudentService - CreateAsync] - Error adding student");
                return Result<string>.Failure($"Error retrieving students: {ex.Message}");
            }
        }

        public async Task<Result<string>> Updatesync(Guid id, StudentUpdateDto studentUpdateDto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(studentUpdateDto, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                    .ToList();

                return Result<string>.ValidationFailure(errors);
            }

            try
            {
                var studentEntity = studentUpdateDto.Adapt<Student>();
                await _studentRepository.UpdateAsync(id, studentEntity, cancellationToken);

                return Result<string>.Success("Student update completed successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error updating students: {ex.Message}");
            }
        }

        public async Task<Result<string>> DaleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _studentRepository.DeleteAsync(id, cancellationToken);
                return Result<string>.Success("Student successfully deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudentService - DeleteAsync] - Error deleting students");
                return Result<string>.Failure($"Error deleting students: {ex.Message}");
            }
        }
    }
}

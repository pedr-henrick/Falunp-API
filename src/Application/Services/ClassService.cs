using Application.Common;
using Application.DTOs.Class;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ClassService(
        ILogger<ClassService> logger,
        IClassRepository classRepository,
        IValidator<ClassDto> classValidator) : IClassService
    {
        private readonly ILogger<ClassService> _logger = logger;
        private readonly IClassRepository _classRepository = classRepository;
        private readonly IValidator<ClassDto> _classValidator = classValidator;

        public async Task<Result<List<ClassInfoDto>>> GetAsync(ClassFilterDto classFilterDto, CancellationToken cancellationToken)
        {
            try
            {
                var classEntity = classFilterDto.Adapt<Class>();
                var classes = await _classRepository.GetAsync(classEntity, cancellationToken);

                var response = classes.Adapt<List<ClassInfoDto>>();
                return Result<List<ClassInfoDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ClassService - GetAsync] - Error retrieving classes");
                return Result<List<ClassInfoDto>>.Failure($"Error retrieving classes: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateAsync(ClassDto ClassDto, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _classValidator.ValidateAsync(ClassDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                        .ToList();
                    return Result<string>.ValidationFailure(errors);
                }

                var classEntity = ClassDto.Adapt<Class>();
                await _classRepository.CreateAsync(classEntity, cancellationToken);
                
                return Result<string>.Success("Class added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ClassService - CreateAsync] - Error adding class");
                return Result<string>.Failure($"Error adding class: {ex.Message}");
            }
        }

        public async Task<Result<string>> UpdateAsync(Guid id, ClassDto ClassDto, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _classValidator.ValidateAsync(ClassDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                        .ToList();
                    return Result<string>.ValidationFailure(errors);
                }

                var classEntity = ClassDto.Adapt<Class>();
                await _classRepository.UpdateAsync(id, classEntity, cancellationToken);

                return Result<string>.Success("Class update completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ClassService - UpdateAsync] - Error updating class");
                return Result<string>.Failure($"Error updating class: {ex.Message}");
            }
        }

        public async Task<Result<string>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _classRepository.DeleteAsync(id, cancellationToken);
                return Result<string>.Success("Class successfully deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ClassService - DeleteAsync] - Error deleting class");
                return Result<string>.Failure($"Error deleting class: {ex.Message}");
            }
        }
    }
}

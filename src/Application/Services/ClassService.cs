using Application.Common;
using Application.DTOs.Class;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Mapster;

namespace Application.Services
{
    public class ClassService(IClassRepository classRepository, IValidator<ClassCreateDto> classInfoValidator) : IClassService
    {
        private readonly IClassRepository _classRepository = classRepository;
        private readonly IValidator<ClassCreateDto> _classInfoValidator = classInfoValidator;

        public async Task<Result<List<ClassInfoDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var classes = await _classRepository.GetAllAsync(cancellationToken);
                var response = classes.Adapt<List<ClassInfoDto>>();

                return Result<List<ClassInfoDto>>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<List<ClassInfoDto>>.Failure($"Error retrieving classes: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateAsync(ClassCreateDto classCreateDto, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _classInfoValidator.ValidateAsync(classCreateDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                        .ToList();
                    return Result<string>.ValidationFailure(errors);
                }

                var classEntity = classCreateDto.Adapt<Class>();
                await _classRepository.CreateAsync(classEntity, cancellationToken);
                
                return Result<string>.Success("Class added successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error adding class: {ex.Message}");
            }
        }
    }
}

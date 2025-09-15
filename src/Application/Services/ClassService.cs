using Application.Common;
using Application.DTOs.Class;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class ClassService(IClassRepository classRepository) : IClassService
    {
        public IClassRepository _classRepository = classRepository;
        
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
    }
}

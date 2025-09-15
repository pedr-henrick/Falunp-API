using Application.Common;
using Application.DTOs.Class;

namespace Application.Interfaces
{
    public interface IClassService
    {
        Task<Result<List<ClassInfoDto>>> GetAsync(ClassFilterDto classFilterDto, CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(ClassDto classInfoDto, CancellationToken cancellationToken);
        Task<Result<string>> UpdateAsync(Guid id, ClassDto ClassDto, CancellationToken cancellationToken);
        Task<Result<string>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
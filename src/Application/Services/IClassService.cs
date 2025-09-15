using Application.Common;
using Application.DTOs.Class;

namespace Application.Services
{
    public interface IClassService
    {
        Task<Result<List<ClassInfoDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(ClassDto classInfoDto, CancellationToken cancellationToken);
        Task<Result<string>> UpdateAsync(Guid id, ClassDto ClassDto, CancellationToken cancellationToken);
        Task<Result<string>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
using Application.Common;
using Application.DTOs.Class;

namespace Application.Services
{
    public interface IClassService
    {
        Task<Result<List<ClassInfoDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(ClassCreateDto classInfoDto, CancellationToken cancellationToken);
    }
}
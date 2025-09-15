using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<List<Class>> GetAllAsync(CancellationToken cancellationToken);
    }
}
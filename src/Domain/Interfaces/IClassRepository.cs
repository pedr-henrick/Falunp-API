using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateAsync(Class classEntity, CancellationToken cancellationToken);
        Task UpdateAsync(Guid id, Class classEntity, CancellationToken cancellationToken);
        Task<List<Class>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
    }
}
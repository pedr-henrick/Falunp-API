using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAsync(Class classEntity, CancellationToken cancellationToken);
        Task CreateAsync(Class classEntity, CancellationToken cancellationToken);
        Task UpdateAsync(Guid id, Class classEntity, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
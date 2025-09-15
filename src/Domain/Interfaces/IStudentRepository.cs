using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync(CancellationToken cancellationToken);
    }
}
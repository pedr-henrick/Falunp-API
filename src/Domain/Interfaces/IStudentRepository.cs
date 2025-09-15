using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetPagedAsync(Student student, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Student> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(Student studentEntity, CancellationToken cancellationToken);
        Task UpdateAsync(Guid id, Student studentEntity, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CpfExistsAsync(string cpf);
    }
}
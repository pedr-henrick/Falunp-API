using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAsync(Student student, CancellationToken cancellationToken);
        Task CreateAsync(Student studentEntity, CancellationToken cancellationToken);
        Task UpdateAsync(Guid id, Student studentEntity, CancellationToken cancellationToken);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CpfExistsAsync(string cpf);
    }
}
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetAsync(CancellationToken cancellationToken);
        Task CreateAsync(Enrollment enrollmentEntity, CancellationToken cancellationToken);
    }
}
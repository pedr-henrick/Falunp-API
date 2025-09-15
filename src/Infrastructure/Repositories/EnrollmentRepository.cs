using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EnrollmentRepository(InfrastructureDbContext dbContext) : IEnrollmentRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Enrollment>> GetAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Class)
            .ToListAsync(cancellationToken);
        }

        public async Task CreateAsync(Enrollment enrollmentEntity, CancellationToken cancellationToken)
        {
            await _dbContext.Enrollments.AddAsync(enrollmentEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteFilteredAsync(Enrollment enrollmentEntity, CancellationToken cancellationToken)
        {
            if (enrollmentEntity.StudentId == default  && enrollmentEntity.ClassId == default)
            {
                throw new ArgumentException("At least one filter (StudentId or ClassId) must be provided.");
            }

            var query = _dbContext.Enrollments.AsQueryable();

            if (enrollmentEntity.StudentId.HasValue)
                query = query.Where(e => e.StudentId == enrollmentEntity.StudentId.Value);
            if (enrollmentEntity.ClassId.HasValue)
                query = query.Where(e => e.ClassId == enrollmentEntity.ClassId.Value);

            var deletedRows = await query.ExecuteDeleteAsync(cancellationToken);
            if (deletedRows == 0)
                throw new Exception("No registrations found for the given filters.");
        }
    }
}

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
    }
}

using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class EnrollmentRepository(InfrastructureDbContext dbContext, ILogger<EnrollmentRepository> logger) : IEnrollmentRepository
    {
        private readonly ILogger<EnrollmentRepository> _logger = logger;
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Enrollment>> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _dbContext.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Class)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentRepository - GetAsync] - Error retrieving enrollments");
                throw new Exception($"Error retrieving enrollments: {ex.Message}");
            }
        }

        public async Task CreateAsync(Enrollment enrollmentEntity, CancellationToken cancellationToken)
        {
            try
            {
                if (await _dbContext.Enrollments.AnyAsync(e => e.StudentId == enrollmentEntity.StudentId && e.ClassId == enrollmentEntity.ClassId, cancellationToken))
                    throw new Exception("Student already enrolled in this class.");

                await _dbContext.Enrollments.AddAsync(enrollmentEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentRepository - CreateAsync] - Error adding enrollment");
                throw new Exception($"Error adding enrollment: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Enrollment enrollmentEntity, CancellationToken cancellationToken)
        {
            try
            {
                var enrollmentDb = await _dbContext.Enrollments.FirstAsync(x => x.ClassId == enrollmentEntity.ClassId && x.StudentId == enrollmentEntity.StudentId, cancellationToken)
                    ?? throw new Exception("Enrollment Not Found");

                enrollmentDb.RegistrationDate = enrollmentEntity.RegistrationDate;

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentRepository - UpdateAsync] - Error updating enrollment");
                throw new Exception($"Error updating enrollment: {ex.Message}");
            }
        }


        public async Task DeleteFilteredAsync(Enrollment enrollmentEntity, CancellationToken cancellationToken)
        {
            try
            {
                if (enrollmentEntity.StudentId == default && enrollmentEntity.ClassId == default)
                {
                    throw new ArgumentException("At least one filter (StudentId or ClassId) must be provided.");
                }

                var query = _dbContext.Enrollments.AsQueryable();

                if (enrollmentEntity.StudentId.HasValue)
                    query = query.Where(e => e.StudentId == enrollmentEntity.StudentId.Value);
                if (enrollmentEntity.ClassId.HasValue)
                    query = query.Where(e => e.ClassId == enrollmentEntity.ClassId.Value);

                var toDelete = await query.ToListAsync(cancellationToken);
                if (toDelete.Count == 0)
                    throw new Exception("No registrations found for the given filters.");

                _dbContext.Enrollments.RemoveRange(toDelete);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnrollmentRepository - UpdateAsync] - Error deleting enrollment");
                throw new Exception($"Error deleting enrollment: {ex.Message}");
            }
        }
    }
}

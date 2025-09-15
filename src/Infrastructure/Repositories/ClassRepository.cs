using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassRepository(InfrastructureDbContext dbContext) : IClassRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Class>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Classes.ToListAsync(cancellationToken);
        }

        public async Task CreateAsync(Class classEntity, CancellationToken cancellationToken)
        {
            await _dbContext.Classes.AddAsync(classEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Guid id, Class classEntity, CancellationToken cancellationToken)
        {
            if (await _dbContext.Classes.AnyAsync(c => c.Name == classEntity.Name && c.Id != id, cancellationToken))
            {
                throw new Exception("The class name is already in use.");
            }

            var classDb = await _dbContext.Classes.FindAsync(id, cancellationToken)
                ?? throw new Exception("Class Not Found");

            classDb.Name = classEntity.Name;
            classDb.Description = classEntity.Description;
            classDb.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("unique constraint") == true)
                {
                    throw new Exception("The class name is already in use.");
                }
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var classDb = await _dbContext.Classes.FindAsync(id, cancellationToken)
                ?? throw new Exception("Class Not Found");

            _dbContext.Classes.Remove(classDb);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error deleting class: " + ex.InnerException?.Message);
            }
        }

        public async Task<List<Class>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await _dbContext.Classes
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
    }
}

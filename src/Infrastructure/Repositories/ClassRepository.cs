using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassRepository(InfrastructureDbContext dbContext) : IClassRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Class>> GetAsync(Class classEntity, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Classes.AsQueryable();

                if (classEntity.Id != null)
                    query = query.Where(s => s.Id == classEntity.Id);
                if (!string.IsNullOrEmpty(classEntity.Name))
                    query = query.Where(s => s.Name.Contains(classEntity.Name));

                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving classes:  {ex.Message}");
            }
        }

        public async Task CreateAsync(Class classEntity, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Classes.AddAsync(classEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding classes: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Guid id, Class classEntity, CancellationToken cancellationToken)
        {
            try
            {
                if (await _dbContext.Classes.AnyAsync(c => c.Name == classEntity.Name && c.Id != id, cancellationToken))
                {
                    throw new Exception("The class name is already in use.");
                }

                var classDb = await _dbContext.Classes.FirstAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Class Not Found");

                classDb.Name = classEntity.Name;
                classDb.Description = classEntity.Description;
                classDb.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating classes: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var classDb = await _dbContext.Classes.FirstAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Class Not Found");
                _dbContext.Classes.Remove(classDb);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting class: {ex.Message}");
            }
        }
    }
}

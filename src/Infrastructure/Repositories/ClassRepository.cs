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

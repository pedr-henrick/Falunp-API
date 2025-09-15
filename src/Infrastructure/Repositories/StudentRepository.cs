using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentRepository(InfrastructureDbContext dbContext) : IStudentRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Students.ToListAsync(cancellationToken);
        }
    }
}

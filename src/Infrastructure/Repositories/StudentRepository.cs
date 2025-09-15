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

        public async Task CreateAsync(Student studentEntity, CancellationToken cancellationToken)
        {
            await _dbContext.Students.AddAsync(studentEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Students.AnyAsync(s => s.Email == email);
        }

        public async Task<bool> CpfExistsAsync(string cpf)
        {
            return await _dbContext.Students.AnyAsync(s => s.CPF == cpf);
        }
    }
}

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class StudentRepository(InfrastructureDbContext dbContext) : IStudentRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Student>> GetAsync(Student student, CancellationToken cancellationToken)
        {
            var query = _dbContext.Students.AsQueryable();

            if (student.Id != null)
                query = query.Where(s => s.Id == student.Id);
            if (!string.IsNullOrEmpty(student.Name))
                query = query.Where(s => s.Name.Contains(student.Name));
            if (!string.IsNullOrEmpty(student.Email))
                query = query.Where(s => s.Email == student.Email);
            if (!string.IsNullOrEmpty(student.CPF))
                query = query.Where(s => s.CPF == student.CPF);

           return await query.ToListAsync(cancellationToken);
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

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

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

        public async Task UpdateAsync(Guid id, Student studentEntity, CancellationToken cancellationToken)
        {
            if (await _dbContext.Students.AnyAsync(c => (c.Email == studentEntity.Email || c.CPF == studentEntity.CPF) && c.Id != id, cancellationToken))
                throw new Exception("There is already a student with the email or CPF you are trying to update.");

            var studentDb = await _dbContext.Students.FirstAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Student Not Found");

            studentDb.Name = studentEntity.Name;
            studentDb.BirthDate = studentEntity.BirthDate;
            studentDb.CPF = studentEntity.CPF;
            studentDb.Email = studentEntity.Email;
            studentDb.UpdatedAt = DateTime.UtcNow;

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

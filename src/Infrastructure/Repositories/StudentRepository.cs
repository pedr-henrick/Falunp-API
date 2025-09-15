using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentRepository(InfrastructureDbContext dbContext) : IStudentRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<List<Student>> GetPagedAsync(Student studentEntity, int page, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Students.AsNoTracking().AsQueryable();

                if (studentEntity.Id.HasValue && studentEntity.Id != Guid.Empty)
                    query = query.Where(s => s.Id == studentEntity.Id);
                if (!string.IsNullOrEmpty(studentEntity.Name))
                    query = query.Where(s => s.Name.Contains(studentEntity.Name));
                if (!string.IsNullOrEmpty(studentEntity.Email))
                    query = query.Where(s => s.Email == studentEntity.Email);
                if (!string.IsNullOrEmpty(studentEntity.CPF))
                    query = query.Where(s => s.CPF == studentEntity.CPF);

                return await query
                    .OrderBy(s => s.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving students: {ex.Message}");
            }
        }

        public async Task<Student> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                return await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
                    ?? throw new Exception("Student Not Found");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving students: {ex.Message}");
            }
        }

        public async Task CreateAsync(Student studentEntity, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Students.AddAsync(studentEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding students: {ex.Message}");
            }
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
            catch (Exception ex)
            {
                throw new Exception($"Error updating students: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {

            try
            {
                var classDb = await GetByIdAsync(id, cancellationToken);
                _dbContext.Students.Remove(classDb);
                
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error deleting student: {ex.Message}");
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

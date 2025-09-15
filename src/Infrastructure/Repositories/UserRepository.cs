using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Commons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository(InfrastructureDbContext dbContext) : IUserRepository
    {
        private readonly InfrastructureDbContext _dbContext = dbContext;

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}

using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace UnitTests.Infrastructure.Repositories
{
    public class UserRepositoryUnitTest
    {
        private readonly UserRepository _userRepository;
        private readonly InfrastructureDbContext _dbContext;

        public UserRepositoryUnitTest()
        {
            var options = new DbContextOptionsBuilder<InfrastructureDbContext>()
                .UseInMemoryDatabase(databaseName: "UserRepositoryTestsDb")
                .Options;

            _dbContext = new InfrastructureDbContext(options);

            // Seed some users
            _dbContext.Users.Add(new User { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com", Password = "hashedpassword" });
            _dbContext.Users.Add(new User { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com", Password = "hashedpassword" });
            _dbContext.SaveChanges();

            _userRepository = new UserRepository(_dbContext);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Act
            var user = await _userRepository.GetByEmailAsync("john@example.com", CancellationToken.None);

            // Assert
            user.ShouldNotBeNull();
            user.Name.ShouldBe("John Doe");
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Act
            var user = await _userRepository.GetByEmailAsync("nonexistent@example.com", CancellationToken.None);

            // Assert
            user.ShouldBeNull();
        }
    }
}

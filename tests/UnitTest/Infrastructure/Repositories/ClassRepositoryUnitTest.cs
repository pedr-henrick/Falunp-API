using Bogus;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace UnitTests.Infrastructure.Repositories
{
    public class ClassRepositoryUnitTest
    {
        private readonly ClassRepository _repository;
        private readonly InfrastructureDbContext _context;

        public ClassRepositoryUnitTest()
        {
            var options = new DbContextOptionsBuilder<InfrastructureDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InfrastructureDbContext(options);

            var loggerMock = new Mock<ILogger<ClassRepository>>();
            _repository = new ClassRepository(_context, loggerMock.Object);
        }

        private Class GenerateClass()
        {
            return new Faker<Class>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, f => f.Commerce.Department())
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .Generate();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddClass()
        {
            // Arrange
            var @class = GenerateClass();

            // Act
            await _repository.CreateAsync(@class, CancellationToken.None);

            // Assert
            var result = await _context.Classes.FirstOrDefaultAsync();
            result.ShouldNotBeNull();
            result.Name.ShouldBe(@class.Name);
            result.Description.ShouldBe(@class.Description);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnClassesFilteredByName()
        {
            // Arrange
            var @class1 = GenerateClass();
            var @class2 = GenerateClass();
            @class2.Name = "SpecialClass";

            await _context.Classes.AddRangeAsync(@class1, @class2);
            await _context.SaveChangesAsync();

            var filter = new Class { Name = "Special" };

            // Act
            var result = await _repository.GetAsync(filter, CancellationToken.None);

            // Assert
            result.Count.ShouldBe(1);
            result.First().Name.ShouldContain("Special");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyClass()
        {
            // Arrange
            var @class = GenerateClass();
            await _repository.CreateAsync(@class, CancellationToken.None);

            var updatedClass = new Class
            {
                Name = "UpdatedName",
                Description = "UpdatedDescription"
            };

            // Act
            await _repository.UpdateAsync(@class.Id.Value, updatedClass, CancellationToken.None);

            // Assert
            var result = await _context.Classes.FirstAsync(c => c.Id == @class.Id);
            result.Name.ShouldBe("UpdatedName");
            result.Description.ShouldBe("UpdatedDescription");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveClass()
        {
            // Arrange
            var @class = GenerateClass();
            await _repository.CreateAsync(@class, CancellationToken.None);

            // Act
            await _repository.DeleteAsync(@class.Id.Value, CancellationToken.None);

            // Assert
            var exists = await _context.Classes.AnyAsync();
            exists.ShouldBeFalse();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNameAlreadyExists()
        {
            // Arrange
            var class1 = GenerateClass();
            var class2 = GenerateClass();

            await _repository.CreateAsync(class1, CancellationToken.None);
            await _repository.CreateAsync(class2, CancellationToken.None);

            var updatedClass = new Class
            {
                Name = class1.Name,
                Description = "Desc"
            };

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
                await _repository.UpdateAsync(class2.Id.Value, updatedClass, CancellationToken.None));
        }
    }
}

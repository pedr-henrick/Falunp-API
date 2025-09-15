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
    public class StudentRepositoryUnitTest
    {
        private readonly StudentRepository _repository;
        private readonly InfrastructureDbContext _context;

        public StudentRepositoryUnitTest()
        {
            var options = new DbContextOptionsBuilder<InfrastructureDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InfrastructureDbContext(options);

            var loggerMock = new Mock<ILogger<StudentRepository>>();
            _repository = new StudentRepository(_context, loggerMock.Object);
        }

        private Student GenerateStudent() =>
            new Faker<Student>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.Name, f => f.Person.FullName)
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.CPF, f => f.Random.Replace("###########"))
                .RuleFor(s => s.BirthDate, f => f.Person.DateOfBirth)
                .RuleFor(s => s.Password, f => "Student@123")
                .Generate();

        [Fact]
        public async Task CreateAsync_ShouldAddStudent()
        {
            // Arrange
            var student = GenerateStudent();

            // Act
            await _repository.CreateAsync(student, CancellationToken.None);

            // Assert
            var studentFromDb = await _context.Students.FindAsync(student.Id);
            studentFromDb.ShouldNotBeNull();
            studentFromDb.Name.ShouldBe(student.Name);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnFilteredStudents()
        {
            // Arrange
            var student1 = GenerateStudent();
            var student2 = GenerateStudent();
            await _repository.CreateAsync(student1, CancellationToken.None);
            await _repository.CreateAsync(student2, CancellationToken.None);

            var filter = new Student { Name = student1.Name };

            // Act
            var result = await _repository.GetPagedAsync(filter, 1, 10, CancellationToken.None);

            // Assert
            result.Count.ShouldBe(1);
            result.First().Name.ShouldBe(student1.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnStudent_WhenExists()
        {
            // Arrange
            var student = GenerateStudent();
            await _repository.CreateAsync(student, CancellationToken.None);

            // Act
            var result = await _repository.GetByIdAsync(student.Id ?? Guid.NewGuid(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(student.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyStudent()
        {
            // Arrange
            var student = GenerateStudent();
            await _repository.CreateAsync(student, CancellationToken.None);

            var updatedStudent = new Student
            {
                Name = "Updated Name",
                Email = student.Email,
                CPF = student.CPF,
                BirthDate = student.BirthDate
            };

            // Act
            await _repository.UpdateAsync(student.Id ?? Guid.NewGuid(), updatedStudent, CancellationToken.None);

            // Assert
            var studentFromDb = await _repository.GetByIdAsync(student.Id ?? Guid.NewGuid(), CancellationToken.None);
            studentFromDb.Name.ShouldBe("Updated Name");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveStudent()
        {
            // Arrange
            var student = GenerateStudent();
            await _repository.CreateAsync(student, CancellationToken.None);

            // Act
            await _repository.DeleteAsync(student.Id ?? Guid.NewGuid(), CancellationToken.None);

            // Assert
            var exists = await _context.Students.AnyAsync(s => s.Id == student.Id);
            exists.ShouldBeFalse();
        }

        [Fact]
        public async Task EmailExistsAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var student = GenerateStudent();
            await _repository.CreateAsync(student, CancellationToken.None);

            // Act
            var result = await _repository.EmailExistsAsync(student.Email);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task CpfExistsAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var student = GenerateStudent();
            await _repository.CreateAsync(student, CancellationToken.None);

            // Act
            var result = await _repository.CpfExistsAsync(student.CPF);

            // Assert
            result.ShouldBeTrue();
        }
    }
}

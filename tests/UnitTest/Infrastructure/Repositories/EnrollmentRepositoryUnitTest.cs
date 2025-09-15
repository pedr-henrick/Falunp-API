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
    public class EnrollmentRepositoryUnitTest
    {
        private readonly EnrollmentRepository _repository;
        private readonly InfrastructureDbContext _context;

        public EnrollmentRepositoryUnitTest()
        {
            var options = new DbContextOptionsBuilder<InfrastructureDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InfrastructureDbContext(options);

            var loggerMock = new Mock<ILogger<EnrollmentRepository>>();
            _repository = new EnrollmentRepository(_context, loggerMock.Object);
        }

        private (Student, Class, Enrollment) GenerateEnrollment()
        {
            var studentFaker = new Faker<Student>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.Name, f => f.Name.FullName())
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.CPF, f => f.Random.ReplaceNumbers("###########"))
                .RuleFor(s => s.BirthDate, f => f.Date.Past(25, DateTime.Now.AddYears(-18)))
                .RuleFor(s => s.Password, f => "Student@123")
                .Generate();

            var classfaker = new Faker<Class>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, f => f.Commerce.Department())
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .Generate();

            var enrollment = new Enrollment
            {
                StudentId = studentFaker.Id,
                ClassId = classfaker.Id,
                RegistrationDate = DateTime.UtcNow,
                Student = studentFaker,
                Class = classfaker
            };

            return (studentFaker, classfaker, enrollment);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEnrollment()
        {
            // Arrange
            var (student, classfaker, enrollment) = GenerateEnrollment();
            await _context.Students.AddAsync(student);
            await _context.Classes.AddAsync(classfaker);
            await _context.SaveChangesAsync();

            // Act
            await _repository.CreateAsync(enrollment, CancellationToken.None);

            // Assert
            var result = await _context.Enrollments.FirstOrDefaultAsync();
            result.ShouldNotBeNull();
            result.StudentId.ShouldBe(student.Id);
            result.ClassId.ShouldBe(classfaker.Id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnAllEnrollments()
        {
            // Arrange
            var (student, classfaker, enrollment) = GenerateEnrollment();
            await _context.Students.AddAsync(student);
            await _context.Classes.AddAsync(classfaker);
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAsync(CancellationToken.None);

            // Assert
            result.Count.ShouldBe(1);
            result.First().StudentId.ShouldBe(student.Id);
            result.First().ClassId.ShouldBe(classfaker.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyEnrollmentDate()
        {
            // Arrange
            var (student, classfaker, enrollment) = GenerateEnrollment();
            await _context.Students.AddAsync(student);
            await _context.Classes.AddAsync(classfaker);
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            var newDate = DateTime.UtcNow.AddDays(1);
            enrollment.RegistrationDate = newDate;

            // Act
            await _repository.UpdateAsync(enrollment, CancellationToken.None);

            // Assert
            var result = await _context.Enrollments.FirstAsync();
            result.RegistrationDate.ShouldBe(newDate);
        }

        [Fact]
        public async Task DeleteFilteredAsync_ShouldRemoveEnrollment_ByStudentId()
        {
            // Arrange
            var (student, classfaker, enrollment) = GenerateEnrollment();
            await _context.Students.AddAsync(student);
            await _context.Classes.AddAsync(classfaker);
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            var filter = new Enrollment { StudentId = student.Id };

            // Act
            await _repository.DeleteFilteredAsync(filter, CancellationToken.None);

            // Assert
            var exists = await _context.Enrollments.AnyAsync();
            exists.ShouldBeFalse();
        }

        [Fact]
        public async Task DeleteFilteredAsync_ShouldDelete_WhenFiltersProvided()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var classId = Guid.NewGuid();

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                ClassId = classId,
                RegistrationDate = DateTime.UtcNow
            };

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteFilteredAsync(new Enrollment { StudentId = studentId }, CancellationToken.None);

            // Assert
            var exists = await _context.Enrollments.AnyAsync(e => e.StudentId == studentId);
            exists.ShouldBeFalse();
        }

    }
}

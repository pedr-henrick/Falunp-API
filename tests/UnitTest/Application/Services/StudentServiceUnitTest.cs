using Application.Common;
using Application.DTOs.Student;
using Application.Interfaces;
using Application.Services;
using Bogus;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace UnitTests.Application.Services
{
    public class StudentServiceUnitTest
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<IValidator<StudentCreateDto>> _createValidatorMock;
        private readonly Mock<IValidator<StudentUpdateDto>> _updateValidatorMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILogger<StudentService>> _loggerMock;
        private readonly Faker<StudentCreateDto> _createFaker;
        private readonly Faker<StudentUpdateDto> _updateFaker;
        private readonly Faker<StudentFilterDto> _filterFaker;
        private readonly StudentService _service;

        public StudentServiceUnitTest()
        {
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _createValidatorMock = new Mock<IValidator<StudentCreateDto>>();
            _updateValidatorMock = new Mock<IValidator<StudentUpdateDto>>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerMock = new Mock<ILogger<StudentService>>();

            _service = new StudentService(
                _loggerMock.Object,
                _studentRepositoryMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _passwordHasherMock.Object
            );

            _createFaker = new Faker<StudentCreateDto>()
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###########"))
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.BirthDate, f => f.Date.Past(30, DateTime.UtcNow.AddYears(-18)))
                .RuleFor(x => x.Password, f => f.Internet.Password());

            _updateFaker = new Faker<StudentUpdateDto>()
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###########"))
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.BirthDate, f => f.Date.Past(30, DateTime.UtcNow.AddYears(-18)));

            _filterFaker = new Faker<StudentFilterDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###########"))
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.PageNumber, f => f.Random.Int(1, 5))
                .RuleFor(x => x.PageSize, f => f.Random.Int(5, 20));
        }

        [Fact]
        public async Task GetAsync_ReturnsSuccess_WithStudents()
        {
            var filterDto = _filterFaker.Generate();
            var studentList = new List<Student>
            {
                new Student { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com", CPF = "12345678901", BirthDate = DateTime.UtcNow.AddYears(-20) }
            };

            _studentRepositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<Student>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentList);

            var result = await _service.GetAsync(filterDto, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Count.ShouldBe(1);
            result.Value[0].Name.ShouldBe("John Doe");
        }

        [Fact]
        public async Task CreateAsync_ReturnsSuccess_WhenValidationPasses()
        {
            var createDto = _createFaker.Generate();
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _passwordHasherMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

            _studentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Student added successfully");
        }

        [Fact]
        public async Task CreateAsync_ReturnsValidationFailure_WhenValidationFails()
        {
            var createDto = _createFaker.Generate();
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "Email is required")
            };
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            var result = await _service.CreateAsync(createDto, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.ValidationErrors.Count.ShouldBe(1);
            result.ValidationErrors[0].PropertyName.ShouldBe("Email");
        }

        [Fact]
        public async Task Updatesync_ReturnsSuccess_WhenValidationPasses()
        {
            var updateDto = _updateFaker.Generate();
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _studentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Student>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.Updatesync(Guid.NewGuid(), updateDto, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Student update completed successfully");
        }

        [Fact]
        public async Task DaleteAsync_ReturnsSuccess_WhenRepositoryDeletesSuccessfully()
        {
            _studentRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.DaleteAsync(Guid.NewGuid(), CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Student successfully deleted");
        }

        [Fact]
        public async Task DaleteAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            _studentRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _service.DaleteAsync(Guid.NewGuid(), CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Error deleting students");
        }
    }
}

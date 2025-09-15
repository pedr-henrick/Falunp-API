using Application.DTOs.Enrollment;
using Application.Services;
using Bogus;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace UnitTests.Application.Services
{
    public class EnrollmentServiceUnitTest
    {
        private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock;
        private readonly Mock<IValidator<EnrollmentCreateDto>> _createValidatorMock;
        private readonly Mock<IValidator<EnrollmentUpdateDto>> _updateValidatorMock;
        private readonly Mock<ILogger<EnrollmentService>> _loggerMock;
        private readonly Faker<EnrollmentCreateDto> _createFaker;
        private readonly Faker<EnrollmentUpdateDto> _updateFaker;
        private readonly Faker<EnrollmentFilterDto> _filterFaker;
        private readonly EnrollmentService _service;

        public EnrollmentServiceUnitTest()
        {
            _enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
            _createValidatorMock = new Mock<IValidator<EnrollmentCreateDto>>();
            _updateValidatorMock = new Mock<IValidator<EnrollmentUpdateDto>>();
            _loggerMock = new Mock<ILogger<EnrollmentService>>();

            _service = new EnrollmentService(
                _loggerMock.Object,
                _enrollmentRepositoryMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object
            );

            _createFaker = new Faker<EnrollmentCreateDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.ClassId, f => f.Random.Guid())
                .RuleFor(x => x.RegistrationDate, f => f.Date.Past());

            _updateFaker = new Faker<EnrollmentUpdateDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.ClassId, f => f.Random.Guid())
                .RuleFor(x => x.RegistrationDate, f => f.Date.Past());

            _filterFaker = new Faker<EnrollmentFilterDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.ClassId, f => f.Random.Guid());
        }

        [Fact]
        public async Task GetAsync_ReturnsSuccess_WhenRepositoryReturnsEnrollments()
        {
            // Arrange
            var enrollmentList = new List<Enrollment>
            {
                new()
                {
                    StudentId = Guid.NewGuid(),
                    ClassId = Guid.NewGuid(),
                    RegistrationDate = DateTime.UtcNow,
                    Student = new Domain.Entities.Student { Name = "John Doe" },
                    Class = new Domain.Entities.Class { Name = "Math" }
                }
            };

            _enrollmentRepositoryMock.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(enrollmentList);

            // Act
            var result = await _service.GetAsync(CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.Count.ShouldBe(1);
            result.Value[0].StudentName.ShouldBe("John Doe");
            result.Value[0].ClassName.ShouldBe("Math");
        }

        [Fact]
        public async Task CreateAsync_ReturnsSuccess_WhenValidationPasses()
        {
            // Arrange
            var createDto = _createFaker.Generate();
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _enrollmentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(createDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Enrollment added successfully");
        }

        [Fact]
        public async Task CreateAsync_ReturnsValidationFailure_WhenValidationFails()
        {
            // Arrange
            var createDto = _createFaker.Generate();
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("StudentId", "StudentId is required")
            };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            // Act
            var result = await _service.CreateAsync(createDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.ValidationErrors.Count.ShouldBe(1);
            result.ValidationErrors[0].PropertyName.ShouldBe("StudentId");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsSuccess_WhenValidationPasses()
        {
            // Arrange
            var updateDto = _updateFaker.Generate();
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _enrollmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(updateDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Enrollment added successfully");
        }

        [Fact]
        public async Task DeleteAsync_ReturnsSuccess_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var filterDto = _filterFaker.Generate();
            _enrollmentRepositoryMock.Setup(r => r.DeleteFilteredAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(filterDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Enrollment successfully deleted");
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var filterDto = _filterFaker.Generate();
            _enrollmentRepositoryMock.Setup(r => r.DeleteFilteredAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.DeleteAsync(filterDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Error deleting enrollment");
        }
    }
}

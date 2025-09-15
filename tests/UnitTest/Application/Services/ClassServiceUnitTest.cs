using Application.DTOs.Class;
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
    public class ClassServiceUnitTest
    {
        private readonly Mock<IClassRepository> _classRepositoryMock;
        private readonly Mock<IValidator<ClassDto>> _validatorMock;
        private readonly Mock<ILogger<ClassService>> _loggerMock;
        private readonly Faker<ClassDto> _classDtoFaker;
        private readonly Faker<ClassFilterDto> _classFilterFaker;
        private readonly ClassService _classService;

        public ClassServiceUnitTest()
        {
            _classRepositoryMock = new Mock<IClassRepository>();
            _validatorMock = new Mock<IValidator<ClassDto>>();
            _loggerMock = new Mock<ILogger<ClassService>>();

            _classService = new ClassService(
                _loggerMock.Object,
                _classRepositoryMock.Object,
                _validatorMock.Object
            );

            _classDtoFaker = new Faker<ClassDto>()
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence());

            _classFilterFaker = new Faker<ClassFilterDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Lorem.Word());
        }

        [Fact]
        public async Task GetAsync_ReturnsSuccess_WhenRepositoryReturnsClasses()
        {
            // Arrange
            var filter = _classFilterFaker.Generate();
            var classList = new List<Class>
            {
                new() { Id = Guid.NewGuid(), Name = "Math", Description = "Math class" }
            };

            _classRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Class>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(classList);

            // Act
            var result = await _classService.GetAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.Count.ShouldBe(1);
            result.Value[0].Name.ShouldBe("Math");
        }

        [Fact]
        public async Task CreateAsync_ReturnsSuccess_WhenValidationPasses()
        {
            // Arrange
            var classDto = _classDtoFaker.Generate();
            _validatorMock.Setup(v => v.ValidateAsync(classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _classRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Class>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _classService.CreateAsync(classDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Class added successfully");
        }

        [Fact]
        public async Task CreateAsync_ReturnsValidationFailure_WhenValidationFails()
        {
            // Arrange
            var classDto = _classDtoFaker.Generate();
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Name", "Name is required")
            };

            _validatorMock.Setup(v => v.ValidateAsync(classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            // Act
            var result = await _classService.CreateAsync(classDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.ValidationErrors.Count.ShouldBe(1);
            result.ValidationErrors[0].PropertyName.ShouldBe("Name");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsSuccess_WhenValidationPasses()
        {
            // Arrange
            var classDto = _classDtoFaker.Generate();
            var id = Guid.NewGuid();

            _validatorMock.Setup(v => v.ValidateAsync(classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _classRepositoryMock.Setup(r => r.UpdateAsync(id, It.IsAny<Class>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _classService.UpdateAsync(id, classDto, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Class update completed successfully");
        }

        [Fact]
        public async Task DeleteAsync_ReturnsSuccess_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            _classRepositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _classService.DeleteAsync(id, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe("Class successfully deleted");
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _classRepositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _classService.DeleteAsync(id, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Error deleting class");
        }
    }
}

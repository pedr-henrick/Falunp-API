using Application.Common;
using Application.DTOs.Enrollment;
using Application.Interfaces;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using WebApi.Controllers;

namespace UnitTests.WebApi.Controllers
{
    public class EnrollmentControllerUnitTests
    {
        private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
        private readonly EnrollmentController _controller;

        private readonly Faker<EnrollmentCreateDto> _createFaker;
        private readonly Faker<EnrollmentUpdateDto> _updateFaker;
        private readonly Faker<EnrollmentFilterDto> _filterFaker;
        private readonly Faker<EnrollmentInfoDto> _infoFaker;

        public EnrollmentControllerUnitTests()
        {
            _enrollmentServiceMock = new Mock<IEnrollmentService>();
            _controller = new EnrollmentController(_enrollmentServiceMock.Object);

            _createFaker = new Faker<EnrollmentCreateDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.ClassId, f => f.Random.Guid())
                .RuleFor(x => x.RegistrationDate, f => f.Date.Past(1));

            _updateFaker = new Faker<EnrollmentUpdateDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.ClassId, f => f.Random.Guid())
                .RuleFor(x => x.RegistrationDate, f => f.Date.Past(1));

            _filterFaker = new Faker<EnrollmentFilterDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.ClassId, f => f.Random.Guid());

            _infoFaker = new Faker<EnrollmentInfoDto>()
                .RuleFor(x => x.StudentId, f => f.Random.Guid())
                .RuleFor(x => x.StudentName, f => f.Name.FullName())
                .RuleFor(x => x.ClassId, f => f.Random.Guid())
                .RuleFor(x => x.ClassName, f => f.Company.CompanyName())
                .RuleFor(x => x.RegistrationDate, f => f.Date.Past(1));
        }

        #region GetAllEnrollmentAsync

        [Fact]
        public async Task GetAllEnrollment_ReturnsOk_WhenServiceSucceeds()
        {
            var enrollments = _infoFaker.Generate(3);
            var result = Result<List<EnrollmentInfoDto>>.Success(enrollments);

            _enrollmentServiceMock
                .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.GetAllEnrollmentAsync(CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(enrollments);
        }

        [Fact]
        public async Task GetAllEnrollment_ReturnsValidationProblem_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("StudentId", "Fild required");

            var actionResult = await _controller.GetAllEnrollmentAsync(CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task GetAllEnrollment_ReturnsUnauthorized_WhenServiceFails()
        {
            var result = Result<List<EnrollmentInfoDto>>.Failure("Error");

            _enrollmentServiceMock
                .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.GetAllEnrollmentAsync(CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion

        #region CreateEnrollmentAsync

        [Fact]
        public async Task CreateEnrollment_ReturnsOk_WhenServiceSucceeds()
        {
            var createDto = _createFaker.Generate();
            var result = Result<string>.Success("Create success");

            _enrollmentServiceMock
                .Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateEnrollmentAsync(createDto, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(result.Value);
        }

        [Fact]
        public async Task CreateEnrollment_ReturnsValidationProblem_WhenServiceReturnsValidationErrors()
        {
            var createDto = _createFaker.Generate();
            var validationErrors = new List<ValidationError> { new("StudentId", "Invalid") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _enrollmentServiceMock
                .Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateEnrollmentAsync(createDto, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task CreateEnrollment_ReturnsUnauthorized_WhenServiceFails()
        {
            var createDto = _createFaker.Generate();
            var result = Result<string>.Failure("Create failed");

            _enrollmentServiceMock
                .Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateEnrollmentAsync(createDto, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion

        #region UpdateEnrollmentAsync

        [Fact]
        public async Task UpdateEnrollment_ReturnsOk_WhenServiceSucceeds()
        {
            var updateDto = _updateFaker.Generate();
            var result = Result<string>.Success("Update success");

            _enrollmentServiceMock
                .Setup(s => s.UpdateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateEnrollmentAsync(updateDto, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(result.Value);
        }

        [Fact]
        public async Task UpdateEnrollment_ReturnsValidationProblem_WhenServiceReturnsValidationErrors()
        {
            var updateDto = _updateFaker.Generate();
            var validationErrors = new List<ValidationError> { new("ClassId", "Invalid") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _enrollmentServiceMock
                .Setup(s => s.UpdateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateEnrollmentAsync(updateDto, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task UpdateEnrollment_ReturnsUnauthorized_WhenServiceFails()
        {
            var updateDto = _updateFaker.Generate();
            var result = Result<string>.Failure("Update failed");

            _enrollmentServiceMock
                .Setup(s => s.UpdateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateEnrollmentAsync(updateDto, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion

        #region DeleteEnrollmentByClassAsync

        [Fact]
        public async Task DeleteEnrollment_ReturnsOk_WhenServiceSucceeds()
        {
            var filter = _filterFaker.Generate();
            var result = Result<string>.Success("Delete success");

            _enrollmentServiceMock
                .Setup(s => s.DeleteAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteEnrollmentByClassAsync(filter, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(result.Value);
        }

        [Fact]
        public async Task DeleteEnrollment_ReturnsValidationProblem_WhenServiceReturnsValidationErrors()
        {
            var filter = _filterFaker.Generate();
            var validationErrors = new List<ValidationError> { new("StudentId", "Invalid") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _enrollmentServiceMock
                .Setup(s => s.DeleteAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteEnrollmentByClassAsync(filter, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task DeleteEnrollment_ReturnsUnauthorized_WhenServiceFails()
        {
            var filter = _filterFaker.Generate();
            var result = Result<string>.Failure("Delete failed");

            _enrollmentServiceMock
                .Setup(s => s.DeleteAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteEnrollmentByClassAsync(filter, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion
    }
}

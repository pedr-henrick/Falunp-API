using Application.Common;
using Application.DTOs.Student;
using Application.Interfaces;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using WebApi.Controllers;

namespace UnitTests.WebApi.Controllers
{
    public class StudentControllerTests
    {
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly StudentController _controller;
        private readonly Faker<StudentFilterDto> _filterFaker;
        private readonly Faker<StudentCreateDto> _createFaker;
        private readonly Faker<StudentUpdateDto> _updateFaker;
        private readonly Faker<StudentInfoDto> _infoFaker;

        public StudentControllerTests()
        {
            _studentServiceMock = new Mock<IStudentService>();
            _controller = new StudentController(_studentServiceMock.Object);

            _filterFaker = new Faker<StudentFilterDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(x => x.PageNumber, 1)
                .RuleFor(x => x.PageSize, 10);

            _createFaker = new Faker<StudentCreateDto>()
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(x => x.BirthDate, f => f.Date.Past(30, DateTime.Today.AddYears(-10)))
                .RuleFor(x => x.Password, f => f.Internet.Password());

            _updateFaker = new Faker<StudentUpdateDto>()
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(x => x.BirthDate, f => f.Date.Past(30, DateTime.Today.AddYears(-10)));

            _infoFaker = new Faker<StudentInfoDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.CPF, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(x => x.BirthDate, f => f.Date.Past(30, DateTime.Today.AddYears(-10)));
        }

        #region GetAllStudentesAsync

        [Fact]
        public async Task GetAllStudents_ReturnsOk_WhenServiceSucceeds()
        {
            var filter = _filterFaker.Generate();
            var students = _infoFaker.Generate(3);
            var result = Result<List<StudentInfoDto>>.Success(students);

            _studentServiceMock
                .Setup(s => s.GetAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.GetAllStudentesAsync(filter, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(students);
        }

        [Fact]
        public async Task GetAllStudents_ReturnsValidationProblem_WhenModelStateInvalid()
        {
            var filter = _filterFaker.Generate();
            _controller.ModelState.AddModelError("Name", "Name is required");

            var actionResult = await _controller.GetAllStudentesAsync(filter, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task GetAllStudents_ReturnsUnauthorized_WhenServiceFails()
        {
            var filter = _filterFaker.Generate();
            var result = Result<List<StudentInfoDto>>.Failure("Query failed");

            _studentServiceMock
                .Setup(s => s.GetAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.GetAllStudentesAsync(filter, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        #endregion

        #region CreateStudentesAsync

        [Fact]
        public async Task CreateStudent_ReturnsOk_WhenServiceSucceeds()
        {
            var createDto = _createFaker.Generate();
            var result = Result<string>.Success("Created success");

            _studentServiceMock
                .Setup(s => s.CreateAsync(It.Is<StudentCreateDto>(
                        r => r.Name == createDto.Name && r.Email == createDto.Email),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateStudentesAsync(createDto, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(result.Value);
        }

        [Fact]
        public async Task CreateStudent_ReturnsValidationProblem_WhenServiceReturnsValidationErrors()
        {
            var createDto = _createFaker.Generate();
            var validationErrors = new List<ValidationError>
            {
                new ValidationError("Name", "Invalid name")
            };
            var result = Result<string>.ValidationFailure(validationErrors);

            _studentServiceMock
                .Setup(s => s.CreateAsync(It.Is<StudentCreateDto>(
                        r => r.Name == createDto.Name && r.Email == createDto.Email),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateStudentesAsync(createDto, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task CreateStudent_ReturnsUnauthorized_WhenServiceFails()
        {
            var createDto = _createFaker.Generate();
            var result = Result<string>.Failure("Create failed");

            _studentServiceMock
                .Setup(s => s.CreateAsync(It.Is<StudentCreateDto>(
                        r => r.Name == createDto.Name && r.Email == createDto.Email),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateStudentesAsync(createDto, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        #endregion

        #region UpdateStudentesAsync

        [Fact]
        public async Task UpdateStudent_ReturnsOk_WhenServiceSucceeds()
        {
            var updateDto = _updateFaker.Generate();
            var id = Guid.NewGuid();
            var result = Result<string>.Success("Updated success");

            _studentServiceMock
                .Setup(s => s.Updatesync(id, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateStudentesAsync(id, updateDto, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(result.Value);
        }

        [Fact]
        public async Task UpdateStudent_ReturnsValidationProblem_WhenServiceReturnsValidationErrors()
        {
            var updateDto = _updateFaker.Generate();
            var id = Guid.NewGuid();
            var validationErrors = new List<ValidationError> { new("Name", "Invalid name") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _studentServiceMock
                .Setup(s => s.Updatesync(id, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateStudentesAsync(id, updateDto, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task UpdateStudent_ReturnsUnauthorized_WhenServiceFails()
        {
            var updateDto = _updateFaker.Generate();
            var id = Guid.NewGuid();
            var result = Result<string>.Failure("Update failed");

            _studentServiceMock
                .Setup(s => s.Updatesync(id, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateStudentesAsync(id, updateDto, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        #endregion

        #region DeleteStudentesAsync

        [Fact]
        public async Task DeleteStudent_ReturnsOk_WhenServiceSucceeds()
        {
            var id = Guid.NewGuid();
            var students = _infoFaker.Generate(2);
            var result = Result<string>.Success("Delete success");

            _studentServiceMock
                .Setup(s => s.DaleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteStudentesAsync(id, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(result.Value);
        }

        [Fact]
        public async Task DeleteStudent_ReturnsValidationProblem_WhenServiceReturnsValidationErrors()
        {
            var id = Guid.NewGuid();
            var validationErrors = new List<ValidationError> { new("Name", "Error") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _studentServiceMock
                .Setup(s => s.DaleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteStudentesAsync(id, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task DeleteStudent_ReturnsUnauthorized_WhenServiceFails()
        {
            var id = Guid.NewGuid();
            var result = Result<string>.Failure("Delete failed");

            _studentServiceMock
                .Setup(s => s.DaleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteStudentesAsync(id, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        #endregion
    }
}

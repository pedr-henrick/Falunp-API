using Application.Common;
using Application.DTOs.Class;
using Application.Interfaces;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using WebApi.Controllers;

namespace UnitTests.WebApi.Controllers
{
    public class ClassControllerUnitTest
    {
        private readonly Mock<IClassService> _classServiceMock;
        private readonly ClassController _controller;

        private readonly Faker<ClassDto> _classFaker;
        private readonly Faker<ClassFilterDto> _filterFaker;
        private readonly Faker<ClassInfoDto> _infoFaker;

        public ClassControllerUnitTest()
        {
            _classServiceMock = new Mock<IClassService>();
            _controller = new ClassController(_classServiceMock.Object);

            _classFaker = new Faker<ClassDto>()
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence());

            _filterFaker = new Faker<ClassFilterDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Lorem.Word());

            _infoFaker = new Faker<ClassInfoDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.StudentCount, f => f.Random.Int(1, 50));
        }

        #region GetAllClassesAsync

        [Fact]
        public async Task GetAllClasses_ReturnsOk_WhenServiceSucceeds()
        {
            var filter = _filterFaker.Generate();
            var classes = _infoFaker.Generate(3);
            var result = Result<List<ClassInfoDto>>.Success(classes);

            _classServiceMock
                .Setup(s => s.GetAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.GetAllClassesAsync(filter, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(classes);
        }

        [Fact]
        public async Task GetAllClasses_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var filter = _filterFaker.Generate();
            _controller.ModelState.AddModelError("Name", "Campo obrigatório");

            var actionResult = await _controller.GetAllClassesAsync(filter, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task GetAllClasses_ReturnsUnauthorized_WhenServiceFails()
        {
            var filter = _filterFaker.Generate();
            var result = Result<List<ClassInfoDto>>.Failure("Erro inesperado");

            _classServiceMock
                .Setup(s => s.GetAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.GetAllClassesAsync(filter, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion

        #region CreateClassAsync

        [Fact]
        public async Task CreateClass_ReturnsOk_WhenServiceSucceeds()
        {
            var classDto = _classFaker.Generate();
            var result = Result<string>.Success("Class created successfully");

            _classServiceMock
                .Setup(s => s.CreateAsync(classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateClassAsync(classDto, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe("Class created successfully");
        }

        [Fact]
        public async Task CreateClass_ReturnsBadRequest_WhenValidationFails()
        {
            var classDto = _classFaker.Generate();
            var validationErrors = new List<ValidationError> { new("Name", "Nome inválido") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _classServiceMock
                .Setup(s => s.CreateAsync(classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateClassAsync(classDto, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task CreateClass_ReturnsUnauthorized_WhenServiceFails()
        {
            var classDto = _classFaker.Generate();
            var result = Result<string>.Failure("Falha ao criar turma");

            _classServiceMock
                .Setup(s => s.CreateAsync(classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.CreateClassAsync(classDto, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion

        #region UpdateClassAsync

        [Fact]
        public async Task UpdateClass_ReturnsOk_WhenServiceSucceeds()
        {
            var id = Guid.NewGuid();
            var classDto = _classFaker.Generate();
            var result = Result<string>.Success("Class updated successfully");

            _classServiceMock
                .Setup(s => s.UpdateAsync(id, classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateClassAsync(id, classDto, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe("Class updated successfully");
        }

        [Fact]
        public async Task UpdateClass_ReturnsBadRequest_WhenValidationFails()
        {
            var id = Guid.NewGuid();
            var classDto = _classFaker.Generate();
            var validationErrors = new List<ValidationError> { new("Description", "Descrição inválida") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _classServiceMock
                .Setup(s => s.UpdateAsync(id, classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateClassAsync(id, classDto, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task UpdateClass_ReturnsUnauthorized_WhenServiceFails()
        {
            var id = Guid.NewGuid();
            var classDto = _classFaker.Generate();
            var result = Result<string>.Failure("Falha ao atualizar turma");

            _classServiceMock
                .Setup(s => s.UpdateAsync(id, classDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.UpdateClassAsync(id, classDto, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion

        #region DeleteClassAsync

        [Fact]
        public async Task DeleteClass_ReturnsOk_WhenServiceSucceeds()
        {
            var id = Guid.NewGuid();
            var result = Result<string>.Success("Class deleted successfully");

            _classServiceMock
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteClassAsync(id, CancellationToken.None);

            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe("Class deleted successfully");
        }

        [Fact]
        public async Task DeleteClass_ReturnsBadRequest_WhenValidationFails()
        {
            var id = Guid.NewGuid();
            var validationErrors = new List<ValidationError> { new("Id", "Id inválido") };
            var result = Result<string>.ValidationFailure(validationErrors);

            _classServiceMock
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteClassAsync(id, CancellationToken.None);

            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task DeleteClass_ReturnsUnauthorized_WhenServiceFails()
        {
            var id = Guid.NewGuid();
            var result = Result<string>.Failure("Falha ao deletar turma");

            _classServiceMock
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var actionResult = await _controller.DeleteClassAsync(id, CancellationToken.None);

            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.Value.ShouldBeOfType<ProblemDetails>();
        }

        #endregion
    }
}


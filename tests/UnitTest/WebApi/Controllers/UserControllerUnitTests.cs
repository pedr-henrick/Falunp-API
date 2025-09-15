using Application.Common;
using Application.DTOs.Login;
using Application.Interfaces;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using WebApi.Controllers;

namespace UnitTests.WebApi.Controllers
{
    public class UserControllerUnitTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly UserController _controller;
        private readonly Faker<LoginRequestDto> _loginRequestFaker;

        public UserControllerUnitTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new UserController(_authServiceMock.Object);

            _loginRequestFaker = new Faker<LoginRequestDto>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Internet.Password());
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAuthenticationIsSuccessful()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var response = new LoginResponseDto { Token = "token", User = null };
            var result = Result<LoginResponseDto>.Success(response);

            _authServiceMock
                .Setup(s => s.AuthenticateAsync(
                    It.Is<LoginRequestDto>(r => r.Email == request.Email && r.Password == request.Password),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.Login(request, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task Login_ReturnsValidationProblem_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            _controller.ModelState.AddModelError("Email", "Email é obrigatório");

            // Act
            var actionResult = await _controller.Login(request, CancellationToken.None);

            // Assert
            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task Login_ReturnsValidationProblem_WhenValidationErrorsExist()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var validationErrors = new List<ValidationError>
            {
                new ("Email", "Email inválido")
            };

            var result = Result<LoginResponseDto>.ValidationFailure(validationErrors);

            _authServiceMock
                .Setup(s => s.AuthenticateAsync(
                    It.Is<LoginRequestDto>(r => r.Email == request.Email && r.Password == request.Password),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.Login(request, CancellationToken.None);

            // Assert
            var validationResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            validationResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            validationResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenAuthenticationFails()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var result = Result<LoginResponseDto>.Failure("Invalid credentials");

            _authServiceMock
                .Setup(s => s.AuthenticateAsync(
                    It.Is<LoginRequestDto>(r => r.Email == request.Email && r.Password == request.Password),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.Login(request, CancellationToken.None);

            // Assert
            var unauthorizedResult = actionResult.ShouldBeOfType<UnauthorizedObjectResult>();
            unauthorizedResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);

        }
    }
}

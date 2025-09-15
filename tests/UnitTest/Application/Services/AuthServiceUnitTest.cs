using Application.DTOs.Login;
using Application.Services;
using Bogus;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace UnitTests.Application.Services
{
    public class AuthServiceUnitTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Faker<LoginRequestDto> _loginRequestFaker;
        private readonly AuthService _authService;

        public AuthServiceUnitTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );

            _loginRequestFaker = new Faker<LoginRequestDto>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Internet.Password());
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var fakeUser = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "John Doe",
                Password = "hashedPassword"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeUser);
            _passwordHasherMock.Setup(p => p.VerifyPassword(request.Password, fakeUser.Password))
                .Returns(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(fakeUser))
                .Returns("fake-jwt-token");

            // Act
            var result = await _authService.AuthenticateAsync(request, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.Token.ShouldBe("fake-jwt-token");
            result.Value.User.Email.ShouldBe(fakeUser.Email);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.User)null);

            // Act
            var result = await _authService.AuthenticateAsync(request, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldBe("User not found");
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFailure_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var fakeUser = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "John Doe",
                Password = "hashedPassword"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeUser);
            _passwordHasherMock.Setup(p => p.VerifyPassword(request.Password, fakeUser.Password))
                .Returns(false);

            // Act
            var result = await _authService.AuthenticateAsync(request, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldBe("Incorrect password");
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authService.AuthenticateAsync(request, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Error during authentication");
        }
    }
}

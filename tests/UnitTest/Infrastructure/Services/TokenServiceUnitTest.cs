using Bogus;
using Domain.Entities;
using Infrastructure.Services;
using Infrastructure.Support.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UnitTests.Infrastructure.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IOptions<TokenOptions>> _mockTokenOptions;
        private readonly TokenService _tokenService;
        private readonly TokenOptions _tokenOptions;
        private readonly Faker<User> _userFaker;

        public TokenServiceTests()
        {
            // Configuração do Bogus para gerar dados fakes
            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.Email, f => f.Person.Email);

            // Configuração das opções do token
            _tokenOptions = new TokenOptions
            {
                SecretKey = "MinhaChaveSuperSecretaComPeloMenos32Caracteres!",
                ExpiryInMinutes = 60,
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            _mockTokenOptions = new Mock<IOptions<TokenOptions>>();
            _mockTokenOptions.Setup(x => x.Value).Returns(_tokenOptions);

            _tokenService = new TokenService(_mockTokenOptions.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            token.ShouldNotBeNullOrEmpty();
            token.ShouldContain("."); // JWT tem 3 partes separadas por pontos
        }

        [Fact]
        public void GenerateToken_ShouldContainCorrectClaims()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _tokenService.GenerateToken(user);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            // Assert
            claims.Select(c => c.Value).ShouldContain(user.Id.ToString());
            claims.Select(c => c.Value).ShouldContain(user.Email);
            claims.Select(c => c.Value).ShouldContain(user.Name);
            claims.Select(c => c.Value).ShouldContain("Admin");
        }

        [Fact]
        public void GenerateToken_ShouldHaveCorrectExpiration()
        {
            // Arrange
            var user = _userFaker.Generate();
            var expectedExpiry = DateTime.UtcNow.AddMinutes(_tokenOptions.ExpiryInMinutes);

            // Act
            var token = _tokenService.GenerateToken(user);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            jwtToken.ValidTo.ShouldBe(expectedExpiry, TimeSpan.FromMinutes(1)); // Tolerância de 1 minuto
        }

        [Fact]
        public void GenerateToken_ShouldHaveCorrectIssuerAndAudience()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _tokenService.GenerateToken(user);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            jwtToken.Issuer.ShouldBe(_tokenOptions.Issuer);
            jwtToken.Audiences.ShouldContain(_tokenOptions.Audience);
        }

        [Fact]
        public void GenerateToken_ShouldBeValidWithCorrectSecretKey()
        {
            // Arrange
            var user = _userFaker.Generate();
            var token = _tokenService.GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenOptions.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _tokenOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _tokenOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Act & Assert
            Should.NotThrow(() =>
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            });
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ShouldGenerateDifferentTokens()
        {
            // Arrange
            var user1 = _userFaker.Generate();
            var user2 = _userFaker.Generate();

            // Act
            var token1 = _tokenService.GenerateToken(user1);
            var token2 = _tokenService.GenerateToken(user2);

            // Assert
            token1.ShouldNotBe(token2);
        }

        [Fact]
        public void GenerateToken_WithNullUser_ShouldThrowArgumentNullException()
        {
            // Arrange
            User nullUser = null;

            // Act & Assert
            Should.Throw<NullReferenceException>(() => _tokenService.GenerateToken(nullUser));
        }

        [Fact]
        public void GenerateToken_WithEmptySecretKey_ShouldThrowException()
        {
            // Arrange
            var invalidOptions = new TokenOptions
            {
                SecretKey = "",
                ExpiryInMinutes = 60,
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            var mockInvalidOptions = new Mock<IOptions<TokenOptions>>();
            mockInvalidOptions.Setup(x => x.Value).Returns(invalidOptions);

            var invalidTokenService = new TokenService(mockInvalidOptions.Object);
            var user = _userFaker.Generate();

            // Act & Assert
            Should.Throw<ArgumentException>(() => invalidTokenService.GenerateToken(user));
        }
    }
}
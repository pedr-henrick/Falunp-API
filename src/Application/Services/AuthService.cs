using Application.Common;
using Application.DTOs.Login;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<LoginResponseDto>> AuthenticateAsync(LoginRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

                if (user == null)
                    return Result<LoginResponseDto>.Failure("User not found");

                if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
                    return Result<LoginResponseDto>.Failure("Incorrect password");

                var token = _tokenService.GenerateToken(user);
                var response = new LoginResponseDto
                {
                    Token = token,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name
                    }
                };

                return Result<LoginResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService - AuthenticateAsync] - Error during authentication");
                return Result<LoginResponseDto>.Failure($"Error during authentication: {ex.Message}");
            }
        }
    }
}

using Application.Common;
using Application.DTOs.Login;
using Domain.Interfaces;

namespace Application.Services
{
    public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService) : IAuthService
    {
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
                return Result<LoginResponseDto>.Failure($"Error during authentication: {ex.Message}");
            }
        }
    }
}

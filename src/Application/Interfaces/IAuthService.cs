using Application.Common;
using Application.DTOs.Login;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> AuthenticateAsync(LoginRequestDto userDto, CancellationToken cancellationToken);
    }
}
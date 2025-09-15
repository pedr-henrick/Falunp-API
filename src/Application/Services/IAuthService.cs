
using Application.Common;
using Application.DTOs;

namespace Application.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> AuthenticateAsync(LoginRequestDto userDto);
    }
}
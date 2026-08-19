using IdentityService.DTOs;
using IdentityService.Features.Auth.DTOs.Requests;
using IdentityService.Features.Auth.DTOs.Responses;
using IdentityService.Shared;

namespace IdentityService.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterUserDto dto);
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto);

        Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<Result> LogoutAsync(LogoutRequestDto request);

    }
}

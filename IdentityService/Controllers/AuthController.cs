using IdentityService.DTOs;
using IdentityService.Features.Auth.DTOs.Requests;
using IdentityService.Features.Auth.DTOs.Responses;
using IdentityService.Features.Auth.Interfaces;
using IdentityService.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
            }

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message = result.Message
                });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDto request)
        {
            var result = await _authService.LogoutAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var response = await _authService.RefreshTokenAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}

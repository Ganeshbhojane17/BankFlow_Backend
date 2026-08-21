using System.Security.Claims;
using CustomerService.Application.Common.Interfaces;

namespace CustomerService.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var value =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(
                        ClaimTypes.NameIdentifier)
                    ?.Value;

            return int.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? Role =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.Role)
            ?.Value;

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .IsAuthenticated
            ?? false;
}
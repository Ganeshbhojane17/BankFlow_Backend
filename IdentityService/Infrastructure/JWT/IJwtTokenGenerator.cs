using IdentityService.Domain.Entities;

namespace IdentityService.Infrastructure.JWT
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken();
    }
}

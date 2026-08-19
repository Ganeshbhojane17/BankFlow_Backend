using IdentityService.Domain.Entities;

namespace IdentityService.Features.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task SaveAsync(
        int userId,
        string token,
        DateTime expiryDate);

    Task<RefreshToken?> GetAsync(
        string token);

    Task RevokeAsync(
        string token);
}
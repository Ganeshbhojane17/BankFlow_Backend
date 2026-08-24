using IdentityService.Domain.Entities;
using System.Data;

namespace IdentityService.Features.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> RegisterAsync(User user, IDbTransaction transaction);
        Task<User?> LoginAsync(string email);

        Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate);
        Task<User?> GetUserByIdAsync(int userId);
    }
}

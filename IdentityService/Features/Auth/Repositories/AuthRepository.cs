using Dapper;
using IdentityService.Data;
using IdentityService.Domain.Entities;
using IdentityService.Features.Auth.Interfaces;
using System.Data;

namespace IdentityService.Features.Auth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _context;
        public AuthRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            //var query = "SELECT * FROM Users WHERE Email = @Email";
            //using var connection = _context.CreateConnection();
            //return await connection.QueryFirstOrDefaultAsync<User>(query, new {Email = email});
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                "usp_User_GetByEmail",
                new
                {
                    Email = email
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "usp_User_GetById",
                new
                {
                    UserId = userId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<User?> LoginAsync(string email)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "usp_User_Login",
                new
          {
                 Email = email
                },
                 commandType: CommandType.StoredProcedure);
        }

        public async Task<int> RegisterAsync(User user, IDbTransaction transaction)
        {
            using var connection = _context.CreateConnection();

            var parameters = new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                user.PasswordHash,
                user.Role
            };
            var userId = await transaction.Connection!.QuerySingleAsync<int>("dbo.usp_User_Register", parameters, transaction: transaction,
              commandType: CommandType.StoredProcedure);

            return userId;
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "usp_Save_RefreshToken",
                new
                {
                    UserId = userId,
                    Token = refreshToken,
                    ExpiryDate = expiryDate
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}

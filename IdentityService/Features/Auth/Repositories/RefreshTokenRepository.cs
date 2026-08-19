using Dapper;

using IdentityService.Data;
using IdentityService.Domain.Entities;
using IdentityService.Features.Auth.Interfaces;

namespace IdentityService.Features.Auth.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DapperContext _context;

    public RefreshTokenRepository(
        DapperContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(
        int userId,
        string token,
        DateTime expiryDate)
    {
        using var connection =
            _context.CreateConnection();

        await connection.ExecuteAsync(
            "usp_Save_RefreshToken",
            new
            {
                UserId = userId,
                Token = token,
                ExpiryDate = expiryDate
            },
            commandType:
            System.Data.CommandType.StoredProcedure);
    }

    public async Task<RefreshToken?> GetAsync(
        string token)
    {
        using var connection =
            _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "usp_RefreshToken_Get",
            new
            {
                Token = token
            },
            commandType:
            System.Data.CommandType.StoredProcedure);
    }

    public async Task RevokeAsync(
        string token)
    {
        using var connection =
            _context.CreateConnection();

        await connection.ExecuteAsync(
            "usp_RefreshToken_Revoke",
            new
            {
                Token = token
            },
            commandType:
            System.Data.CommandType.StoredProcedure);
    }
}
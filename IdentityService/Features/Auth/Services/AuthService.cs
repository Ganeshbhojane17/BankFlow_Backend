using Dapper;
using IdentityService.Configuration;
using IdentityService.Data;
using IdentityService.Domain.Entities;
using IdentityService.DTOs;
using IdentityService.Features.Auth.DTOs.Requests;
using IdentityService.Features.Auth.DTOs.Responses;
using IdentityService.Features.Auth.Interfaces;
using IdentityService.Infrastructure.JWT;
using IdentityService.Infrastructure.Messaging;
using IdentityService.Infrastructure.Password;
using IdentityService.Shared;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json;


namespace IdentityService.Features.Auth.Services
{
    public class AuthService: IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtOptions _jwtOptions;    
        private readonly DapperContext _dbContext;
        private readonly IOutboxRepository _outboxRepository;
        public AuthService(IAuthRepository authRepository, IRefreshTokenRepository refreshTokenRepository, 
            IJwtTokenGenerator jwt, IPasswordHasher passwordHasher, IOptions<JwtOptions> jwtOptions,
            DapperContext dbContext, IOutboxRepository outboxRepository)
        {
            _authRepository = authRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwt = jwt;
            _passwordHasher = passwordHasher;
            _jwtOptions = jwtOptions.Value;
            _dbContext = dbContext;
            _outboxRepository = outboxRepository;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var user = await _authRepository.LoginAsync(dto.Email);
            if (user == null)
            {
                return Result<LoginResponseDto>.Failure("Invalid email or password.");
            }
            if (!user.IsActive)
            {
                return Result<LoginResponseDto>.Failure("User account is inactive.");
            }
            var passwordValid = _passwordHasher.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid)
            {
                return Result<LoginResponseDto>.Failure("Invalid email or password.");
            }

            var accessToken = _jwt.GenerateAccessToken(user);

            var refreshToken = _jwt.GenerateRefreshToken();

            await _authRepository.SaveRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays));

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Result<LoginResponseDto>.Ok(
                response,
                "Login successful.");
        }


        public async Task<Result> LogoutAsync(LogoutRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Result.Failure("Refresh token is required.");
            }

            var token = await _refreshTokenRepository.GetAsync(request.RefreshToken);

            if (token == null)
            {
                return Result.Failure("Invalid refresh token.");
            }

            await _refreshTokenRepository.RevokeAsync(request.RefreshToken);

            return Result.Ok("Logout successful.");
        }
        public async Task<Result> RegisterAsync(RegisterUserDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if(existingUser != null)
            {
                return Result.Failure("Email already exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = "Customer"
            };
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 1. Create User
                var userId = await _authRepository.RegisterAsync(user, transaction);
                user.Id = userId;
                // 2. Create event
                var customerRegisteredEvent = new CustomerRegisteredEvent
                    {
                        UserId = userId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    };

                // 3. Serialize event
                var payload = JsonSerializer.Serialize(customerRegisteredEvent);

                // 4. Create Outbox message
                var outboxMessage = new OutboxMessage
                    {
                        EventId = Guid.NewGuid(),
                        EventType = nameof(CustomerRegisteredEvent),
                        RoutingKey = "customer.registered",
                        Payload = payload,
                        CreatedOn = DateTime.UtcNow
                    };

                // 5. Save Outbox message
                await _outboxRepository.AddAsync(outboxMessage, transaction);
                // 6. Commit both
                transaction.Commit();
                return Result.Ok("User registered successfully");
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
        }

        public async Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(
    RefreshTokenRequestDto request)
        {
            var token = await _refreshTokenRepository.GetAsync(request.RefreshToken);

            if (token == null)
            {
                return Result<RefreshTokenResponseDto>.Failure(
                    "Invalid or expired refresh token.");
            }

            var user = await _authRepository.GetUserByIdAsync(token.UserId);

            if (user == null)
            {
                return Result<RefreshTokenResponseDto>.Failure(
                    "User not found.");
            }

            var newAccessToken = _jwt.GenerateAccessToken(user);

            var newRefreshToken = _jwt.GenerateRefreshToken();

            await _refreshTokenRepository.RevokeAsync(request.RefreshToken);

            await _refreshTokenRepository.SaveAsync(
                user.Id,
                newRefreshToken,
                DateTime.UtcNow.AddDays(7));

            return Result<RefreshTokenResponseDto>.Ok(
                new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                },
                "Token refreshed successfully.");
        }


    }
}


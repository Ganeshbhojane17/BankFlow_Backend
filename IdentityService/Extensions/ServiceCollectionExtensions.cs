using IdentityService.Configuration;
using IdentityService.Data;
using IdentityService.Features.Auth.Interfaces;
using IdentityService.Features.Auth.Repositories;
using IdentityService.Features.Auth.Services;
using IdentityService.Infrastructure.JWT;
using IdentityService.Infrastructure.Messaging;
using IdentityService.Infrastructure.Password;

namespace IdentityService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddHostedService<OutboxProcessor>();

            return services;
        }

        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(
                configuration.GetSection(DatabaseOptions.SectionName));

            services.AddSingleton<DapperContext>();

            //RabbitMQ
            services.Configure<RabbitMqOptions>( configuration.GetSection("RabbitMq"));

            services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
            services.AddScoped<UserProvisioningConsumer>();
            services.AddHostedService<RabbitMqConsumerService>();

            return services;
        }
    }
}

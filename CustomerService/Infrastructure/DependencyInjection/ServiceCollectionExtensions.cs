using CustomerService.Application.Common.Interfaces;
using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Application.Features.Customers.Services;
using CustomerService.Application.Features.Dashboard.Interfaces;
using CustomerService.Application.Features.Dashboard.Services;
using CustomerService.Infrastructure.FileStorage;
using CustomerService.Infrastructure.Messaging;
using CustomerService.Infrastructure.Persistence.Dapper;
using CustomerService.Infrastructure.Persistence.Repositories;
using CustomerService.Infrastructure.Security;

namespace CustomerService.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<DapperContext>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<ICustomerService, CustomerServices>();
            services.AddScoped<IFileStorageService,LocalFileStorageService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
            services.AddScoped<CustomerRegisteredConsumer>();
            services.AddHostedService<RabbitMqConsumerService>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
            services.AddHostedService<OutboxProcessor>();
            services.AddScoped<UserCreatedConsumer>();


            return services;
        }
    }
}

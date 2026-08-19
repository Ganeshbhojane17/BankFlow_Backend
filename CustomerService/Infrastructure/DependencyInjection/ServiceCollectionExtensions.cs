using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Infrastructure.Persistence.Dapper;
using CustomerService.Infrastructure.Persistence.Repositories;
using CustomerService.Application.Features.Customers.Services;

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

            return services;
        }
    }
}

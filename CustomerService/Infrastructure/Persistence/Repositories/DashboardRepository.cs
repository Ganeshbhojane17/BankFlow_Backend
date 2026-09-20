using System;
using System.Threading.Tasks;
using CustomerService.Application.Features.Dashboard.DTOs;
using CustomerService.Application.Features.Dashboard.Interfaces;
using CustomerService.Infrastructure.Persistence.Dapper;
using Dapper;

namespace CustomerService.Infrastructure.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DapperContext _dbContext;
        public DashboardRepository(DapperContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var result = await connection.QuerySingleAsync<DashboardSummaryResponse>(
                 "dbo.usp_Dashboard_GetSummary", commandType: System.Data.CommandType.StoredProcedure
                );

            return result;
        }

        public async Task<IEnumerable<RecentCustomerResponse>>GetRecentCustomersAsync(int count)
        {
            using var connection = _dbContext.CreateConnection();

            var parameters = new
            {
                Count = count
            };

            return await connection.QueryAsync<RecentCustomerResponse>(
                "dbo.usp_Dashboard_GetRecentCustomers",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

using CustomerService.Application.Common.Caching;
using CustomerService.Application.Common.Interfaces;
using CustomerService.Application.Features.Dashboard.DTOs;
using CustomerService.Application.Features.Dashboard.Interfaces;
using CustomerService.Configuration;
using Microsoft.Extensions.Options;

namespace CustomerService.Application.Features.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ICacheService _cacheService;

        private readonly CacheSettings _cacheSettings;

        public DashboardService(IDashboardRepository dashboardRepository,
            ICacheService cacheService, IOptions<CacheSettings> cacheSettings)
        {
            _dashboardRepository = dashboardRepository;
            _cacheService = cacheService;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            const string cacheKey = CacheKeys.DashboardSummary;
            // 1. Try Redis
            var cachedSummary = await _cacheService.GetAsync<DashboardSummaryResponse>(cacheKey);
            if (cachedSummary != null)
            {
                return cachedSummary;
            }

            // 2. Cache miss → SQL Server
            var summary = await _dashboardRepository.GetSummaryAsync();
            // 3. Store result in Redis
            await _cacheService.SetAsync(cacheKey, summary, TimeSpan.FromMinutes(
                _cacheSettings.DashboardSummaryExpirationMinutes));

            return summary;
        }

        public async Task<IEnumerable<RecentCustomerResponse>>GetRecentCustomersAsync(int count)
        {
            var cacheKey = CacheKeys.RecentCustomers(count);
            // 1. Try Redis
            var cachedCustomers = await _cacheService.GetAsync<List<RecentCustomerResponse>>(cacheKey);

            if (cachedCustomers != null)
            {
                return cachedCustomers;
            }
            // 2. Cache miss → SQL Server
            var customers =
            (
                await _dashboardRepository.GetRecentCustomersAsync(count)
            ).ToList();

            await _cacheService.SetAsync(cacheKey, customers, TimeSpan.FromMinutes(
               _cacheSettings.RecentCustomersExpirationMinutes));
            // 4. Return result

            return customers;
        }
    }
}

using CustomerService.Application.Features.Dashboard.DTOs;
using CustomerService.Application.Features.Dashboard.Interfaces;

namespace CustomerService.Application.Features.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            return await _dashboardRepository.GetSummaryAsync();
        }

        public async Task<IEnumerable<RecentCustomerResponse>>GetRecentCustomersAsync(int count)
        {
            return await _dashboardRepository.GetRecentCustomersAsync(count);
        }
    }
}

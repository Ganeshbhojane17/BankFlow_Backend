using CustomerService.Application.Features.Dashboard.DTOs;

namespace CustomerService.Application.Features.Dashboard.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryResponse> GetSummaryAsync();
        Task<IEnumerable<RecentCustomerResponse>> GetRecentCustomersAsync(int count);
    }
}

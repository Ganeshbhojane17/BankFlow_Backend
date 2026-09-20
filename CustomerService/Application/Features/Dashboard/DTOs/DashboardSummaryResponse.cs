namespace CustomerService.Application.Features.Dashboard.DTOs
{
    public class DashboardSummaryResponse
    {
        public int TotalCustomers { get; set; }

        public int ActiveCustomers { get; set; }

        public int InactiveCustomers { get; set; }
    }
}

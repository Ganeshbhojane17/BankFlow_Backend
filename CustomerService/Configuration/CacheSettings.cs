namespace CustomerService.Configuration
{
    public class CacheSettings
    {
        public int DashboardSummaryExpirationMinutes { get; set; } = 5;

        public int RecentCustomersExpirationMinutes { get; set; } = 5;
    }
}

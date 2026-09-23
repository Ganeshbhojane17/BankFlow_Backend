namespace CustomerService.Application.Common.Caching
{
    public static class CacheKeys
    {
        public const string DashboardSummary = "dashboard:summary";

        public static string RecentCustomers(int count)
        {
            return $"dashboard:recent-customers:{count}";
        }
    }
}

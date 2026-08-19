namespace CustomerService.Shared.Helpers
{
    public static class CustomerNumberGenerator
    {
        public static string Generate()
        {
            return $"CUST{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
}

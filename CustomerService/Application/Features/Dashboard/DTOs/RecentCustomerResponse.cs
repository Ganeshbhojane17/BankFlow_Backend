namespace CustomerService.Application.Features.Dashboard.DTOs
{
    public class RecentCustomerResponse
    {
        public int Id { get; set; }

        public string CustomerNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}

namespace CustomerService.Application.Features.Customers.DTOs.Responses
{
    public class CustomerResponse
    {
        public int Id { get; set; }

        public string CustomerNumber { get; set; } = "";

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";
    }
}

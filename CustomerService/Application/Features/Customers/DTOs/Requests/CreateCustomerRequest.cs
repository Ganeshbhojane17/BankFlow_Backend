namespace CustomerService.Application.Features.Customers.DTOs.Requests
{
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? PANNumber { get; set; }

        public string? AadhaarNumber { get; set; }

        public string? Occupation { get; set; }

        public decimal? AnnualIncome { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? PostalCode { get; set; }
    }
}

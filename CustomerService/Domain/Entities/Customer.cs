using CustomerService.Domain.Common;

namespace CustomerService.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string CustomerNumber { get; set; } = string.Empty;
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

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

        public bool IsActive { get; set; }
        public string? ProfileImagePath { get; set; }

        public string? DocumentPath { get; set; }

        public string? DocumentName { get; set; }

        public string? DocumentContentType { get; set; }
    }
}

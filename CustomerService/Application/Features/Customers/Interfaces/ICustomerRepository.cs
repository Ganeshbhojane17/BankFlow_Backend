using CustomerService.Domain.Entities;
using CustomerService.Shared.Pagination;

namespace CustomerService.Application.Features.Customers.Interfaces
{
    public interface ICustomerRepository
    {
        Task<int> CreateAsync(Customer customer);

        Task<Customer?> GetByEmailAsync(string email);

        Task<Customer?> GetByCustomerNumberAsync(string customerNumber);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByUserIdAsync(int userId);
        Task<PagedResponse<Customer>>GetAllAsync(PagedRequest request);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id, string modifiedBy);
        Task<Customer?> GetByEmailExceptIdAsync(string email, int id);
        Task ChangeStatusAsync(int id, bool isActive, string modifiedBy);
    }
}

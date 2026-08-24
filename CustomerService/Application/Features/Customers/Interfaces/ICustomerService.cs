using CustomerService.Application.Features.Customers.DTOs.Requests;
using CustomerService.Application.Features.Customers.DTOs.Responses;
using CustomerService.Shared;
using CustomerService.Shared.Pagination;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Features.Customers.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request);
        Task<Result<CustomerResponse>> GetByIdAsync(int id);
        Task<Result<Customer?>> GetMyProfileAsync();
        Task<Result<PagedResponse<CustomerResponse>>> GetAllAsync(PagedRequest request);
        Task<Result> DeleteAsync(int id);
        Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request);
        Task<Result> ChangeStatusAsync(int id, ChangeCustomerStatusRequest request);
        Task CreateFromRegistrationAsync(CustomerRegisteredEvent customerEvent);
    }
}           
        
using AutoMapper;
using CustomerService.Application.Features.Customers.DTOs.Requests;
using CustomerService.Application.Features.Customers.DTOs.Responses;
using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Shared;
using CustomerService.Shared.Helpers;
using CustomerService.Shared.Pagination;

namespace CustomerService.Application.Features.Customers.Services
{
    public class CustomerServices : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        private readonly IMapper _mapper;

        public CustomerServices(
            ICustomerRepository repository,
            IMapper mapper)
        {
            _repository = repository;

            _mapper = mapper;
        }

        public async Task<Result<CustomerResponse>> CreateAsync(
            CreateCustomerRequest request)
        {
            // Check duplicate email

            var existingCustomer =
                await _repository.GetByEmailAsync(request.Email);

            if (existingCustomer != null)
            {
                return Result<CustomerResponse>.Failure(
                    "Customer email already exists.");
            }

            // Map DTO to Entity

            var customer =
                _mapper.Map<Customer>(request);

            // Generate Customer Number

            customer.CustomerNumber =
                CustomerNumberGenerator.Generate();

            customer.CreatedBy = "System";

            // Save

            customer.Id =
                await _repository.CreateAsync(customer);

            // Map Entity to Response

            var response =
                _mapper.Map<CustomerResponse>(customer);

            return Result<CustomerResponse>.Ok(
                response,
                "Customer created successfully.");
        }

        public async Task<Result<CustomerResponse>> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                return Result<CustomerResponse>.Failure(
                    "Customer not found.");
            }

            var response =
                _mapper.Map<CustomerResponse>(customer);

            return Result<CustomerResponse>.Ok(
                response,
                "Customer found.");
        }

        public async Task<Result<PagedResult<CustomerResponse>>> GetAllAsync(PagedRequest request)
        {
            var result = await _repository.GetAllAsync(request);

            var response = _mapper.Map<List<CustomerResponse>>(result.Items);

            return Result<PagedResult<CustomerResponse>>
            .Ok(
                new PagedResult<CustomerResponse>
                {
                    Items = response,

                    PageNumber = result.PageNumber,

                    PageSize = result.PageSize,

                    TotalRecords = result.TotalRecords
                },
                "Customers retrieved successfully.");
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
            {
                return Result.Failure(
                    "Customer not found.");
            }
            await _repository.DeleteAsync(id,"System");
            return Result.Ok(
                "Customer deleted successfully.");
        }

        public async Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request)
        {
            // Step 1 : Check customer exists
           var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                return Result<CustomerResponse>.Failure(
                    "Customer not found.");
            }

            // Step 2 : Check duplicate email

            var duplicateCustomer =
                await _repository.GetByEmailExceptIdAsync(request.Email, id);

            if (duplicateCustomer != null)
            {
                return Result<CustomerResponse>.Failure(
                    "Email already exists.");
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.DateOfBirth = request.DateOfBirth;
            customer.Gender = request.Gender;
            customer.PANNumber = request.PANNumber;
            customer.AadhaarNumber = request.AadhaarNumber;
            customer.Occupation = request.Occupation;
            customer.AnnualIncome = request.AnnualIncome;
            customer.Address = request.Address;
            customer.City = request.City;
            customer.State = request.State;
            customer.Country = request.Country;
            customer.PostalCode = request.PostalCode;
            customer.IsActive = request.IsActive;
            customer.ModifiedDate = DateTime.UtcNow;
            customer.ModifiedBy = "System";   // Later replace with Logged-in User

            // Step 4 : Save
            await _repository.UpdateAsync(customer);
            // Step 5 : Return Response
            var response = _mapper.Map<CustomerResponse>(customer);

            return Result<CustomerResponse>.Ok(
                response,
                "Customer updated successfully.");
        }

        public async Task<Result> ChangeStatusAsync(int id, ChangeCustomerStatusRequest request)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                return Result.Failure(
                    "Customer not found.");
            }

            await _repository.ChangeStatusAsync(id, request.IsActive, "System");

            return Result.Ok(
                request.IsActive
                    ? "Customer activated successfully."
                    : "Customer deactivated successfully.");
        }
    }
}

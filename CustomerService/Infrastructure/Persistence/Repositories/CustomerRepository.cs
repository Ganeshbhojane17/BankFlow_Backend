using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence.Dapper;
using CustomerService.Shared.Pagination;
using Dapper;
using System.Data;

namespace CustomerService.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Customer customer)
        {
            using var connection = _context.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                "usp_Customer_Create",
                new
                {
                    customer.CustomerNumber,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.PhoneNumber,
                    customer.DateOfBirth,
                    customer.Gender,
                    customer.PANNumber,
                    customer.AadhaarNumber,
                    customer.Occupation,
                    customer.AnnualIncome,
                    customer.Address,
                    customer.City,
                    customer.State,
                    customer.Country,
                    customer.PostalCode,
                    customer.CreatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Customer>(
                "usp_Customer_GetByEmail",
                new
                {
                    Email = email
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Customer?> GetByCustomerNumberAsync(string customerNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Customer>(
                "usp_Customer_GetById",
                new
                {
                    Id = id
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<PagedResult<Customer>> GetAllAsync(PagedRequest request)
        {
            using var connection = _context.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                    "usp_Customer_GetAll",
                    new
                    {
                        request.PageNumber,
                        request.PageSize,
                        request.Search,
                        request.SortBy,
                        request.SortDirection
                    },
                    commandType: CommandType.StoredProcedure);

            var customers = await multi.ReadAsync<Customer>();
            var total = await multi.ReadSingleAsync<int>();

            return new PagedResult<Customer>
            {
                Items = customers,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        public async Task UpdateAsync(Customer customer)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "usp_Customer_Update",
                new
                {
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.PhoneNumber,
                    customer.DateOfBirth,
                    customer.Gender,
                    customer.PANNumber,
                    customer.AadhaarNumber,
                    customer.Occupation,
                    customer.AnnualIncome,
                    customer.Address,
                    customer.City,
                    customer.State,
                    customer.Country,
                    customer.PostalCode,
                    customer.IsActive,
                    customer.ModifiedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id, string modifiedBy)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "usp_Customer_Delete",
                new
                {
                    Id = id,
                    ModifiedBy = modifiedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Customer?> GetByEmailExceptIdAsync(string email, int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Customer>(
                "usp_Customer_GetByEmailExceptId",
                new
                {
                    Email = email,
                    Id = id
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task ChangeStatusAsync(int id, bool isActive, string modifiedBy)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "usp_Customer_ChangeStatus",
                new
                {
                    Id = id,
                    IsActive = isActive,
                    ModifiedBy = modifiedBy
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}

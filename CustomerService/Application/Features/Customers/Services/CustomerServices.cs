using AutoMapper;
using CustomerService.Application.Common.Interfaces;
using CustomerService.Application.Features.Customers.DTOs.Requests;
using CustomerService.Application.Features.Customers.DTOs.Responses;
using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.FileStorage;
using CustomerService.Infrastructure.Persistence.Dapper;
using CustomerService.Infrastructure.Persistence.Repositories;
using CustomerService.Shared;
using CustomerService.Shared.Helpers;
using CustomerService.Shared.Pagination;
using Shared.Contracts.Events;
using System.Text.Json;

namespace CustomerService.Application.Features.Customers.Services
{
    public class CustomerServices : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly ILogger<CustomerServices> _logger;
        private readonly IOutboxRepository _outboxRepository;
        private readonly DapperContext _context;
        private readonly ICacheService _cacheService;

        public CustomerServices(ICustomerRepository repository, ICurrentUserService currentUser,
            IMapper mapper, IFileStorageService fileStorage, ILogger<CustomerServices> logger, 
            IOutboxRepository outboxRepository, DapperContext context, ICacheService cacheService)
        {
            _repository = repository;
            _currentUser = currentUser;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _logger = logger;
            _outboxRepository = outboxRepository;
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Result<Customer>> CreateAsync(CreateCustomerRequest request)
        {
            string? profileImagePath = null;
            string? documentPath = null;
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {

                if (request.ProfileImage != null && !FileValidation.IsValidProfileImage(
                        request.ProfileImage))
                {
                    return Result<Customer>.Failure("Invalid profile image.");
                }

                if (request.Document != null && !FileValidation.IsValidDocument(
                        request.Document))
                {
                    return Result<Customer>.Failure("Invalid document.");
                }

                var customer = new Customer
                {
                    CustomerNumber = CustomerNumberGenerator.Generate(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,

                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    PANNumber = request.PANNumber,
                    AadhaarNumber = request.AadhaarNumber,
                    Occupation = request.Occupation,
                    AnnualIncome = request.AnnualIncome,

                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    PostalCode = request.PostalCode,

                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                // Save Profile Image
                if (request.ProfileImage != null)
                {
                    profileImagePath = await _fileStorage.SaveAsync(request.ProfileImage, "profile");
                    customer.ProfileImagePath = profileImagePath;
                }
                // 5. Save PDF document

                if (request.Document != null)
                {
                    documentPath = await _fileStorage.SaveAsync(request.Document, "documents");
                    customer.DocumentPath = documentPath;
                    customer.DocumentName = request.Document.FileName;
                    customer.DocumentContentType = request.Document.ContentType;
                }
                // 6. Insert Customer
                var customerId = await _repository.CreateAsync(customer, connection, transaction);

                // As new customer added, remove data from cache
                await _cacheService.RemoveDashboardCachesAsync();
                customer.Id = customerId;
                // 7. Create User Provisioning Event
                var userEvent = new UserProvisioningRequested
                    {
                        CustomerId = customer.Id,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email
                    };
                var payload = JsonSerializer.Serialize(userEvent);
                // 9. Create Outbox message
                var outboxMessage = new OutboxMessage
                {
                    EventId = Guid.NewGuid(),
                    EventType = nameof(UserProvisioningRequested),
                    RoutingKey ="user.provisioning.requested",
                    Payload = payload,
                    CreatedOn = DateTime.UtcNow
                };
                // 10. Insert Outbox

                await _outboxRepository.AddAsync(outboxMessage, connection, transaction);
                // 11. Commit transaction
                transaction.Commit();
                return Result<Customer>.Ok(customer, "Customer created successfully.");
            }
            catch (Exception ex)
            {
                // Rollback database
                transaction.Rollback();
                // Delete uploaded files
                if (!string.IsNullOrWhiteSpace(profileImagePath))
                {
                    await _fileStorage.DeleteAsync(profileImagePath);
                }

                if (!string.IsNullOrWhiteSpace(documentPath))
                {
                    await _fileStorage.DeleteAsync(documentPath);
                }
                _logger.LogError(ex,"Failed to create customer.");

                return Result<Customer>.Failure("Unable to create customer.");
            }
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

        public async Task<Result<PagedResponse<CustomerResponse>>> GetAllAsync(PagedRequest request)
        {
            request.PageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            request.PageSize = request.PageSize < 1 ? 10 : request.PageSize;

            request.PageSize = request.PageSize > 100 ? 100 : request.PageSize;
            var result = await _repository.GetAllAsync(request);

            var response = _mapper.Map<List<CustomerResponse>>(result.Items);

            return Result<PagedResponse<CustomerResponse>>
            .Ok(
                new PagedResponse<CustomerResponse>
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
            // As new customer added, remove data from cache
            await _cacheService.RemoveDashboardCachesAsync();
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

            // 2. Validate new profile image
            if (request.ProfileImage != null)
            {
                if (!FileValidation.IsValidProfileImage(
                        request.ProfileImage))
                {
                    return Result<CustomerResponse>.Failure(
                        "Invalid profile image.");
                }
            }

            // 3. Validate new document
            if (request.Document != null)
            {
                if (!FileValidation.IsValidDocument(
                        request.Document))
                {
                    return Result<CustomerResponse>.Failure(
                        "Invalid document.");
                }
            }
            // Keep old file paths
            var oldProfileImagePath = customer.ProfileImagePath;
            var oldDocumentPath = customer.DocumentPath;

            string? newProfileImagePath = null;
            string? newDocumentPath = null;
            try
            {
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
                customer.ModifiedDate = DateTime.UtcNow;
                // 5. Save NEW profile image
                if (request.ProfileImage != null)
                {
                    newProfileImagePath = await _fileStorage.SaveAsync(request.ProfileImage, "profile");
                    customer.ProfileImagePath = newProfileImagePath;
                }
                // 6. Save NEW document

                if (request.Document != null)
                {
                    newDocumentPath = await _fileStorage.SaveAsync(request.Document, "documents");
                    customer.DocumentPath = newDocumentPath;
                    customer.DocumentName = request.Document.FileName;
                    customer.DocumentContentType = request.Document.ContentType;
                }
                customer.ModifiedBy = "System";   // Later replace with Logged-in User

                // Step 4 : Save
                await _repository.UpdateAsync(customer);

                // As new customer added, remove data from cache
                await _cacheService.RemoveDashboardCachesAsync();

                // Step 5 : Return Response
                var response = _mapper.Map<CustomerResponse>(customer);

                return Result<CustomerResponse>.Ok(
                    response,
                    "Customer updated successfully.");
            }
            catch
            {
                // DB failed after new file was uploaded

                if (!string.IsNullOrWhiteSpace(newProfileImagePath))
                {
                    await _fileStorage.DeleteAsync(newProfileImagePath);
                }

                if (!string.IsNullOrWhiteSpace(newDocumentPath))
                {
                    await _fileStorage.DeleteAsync(newDocumentPath);
                }
                throw;
            }
            
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

        public async Task<Result<Customer>> GetMyProfileAsync()
        {
            if (!_currentUser.IsAuthenticated)
            {
                return Result<Customer>.Failure(
                    "User is not authenticated.");
            }

            if (!_currentUser.UserId.HasValue)
            {
                return Result<Customer>.Failure(
                    "User identity could not be determined.");
            }

            var customer = await _repository.GetByUserIdAsync(_currentUser.UserId.Value);

            if (customer == null)
            {
                return Result<Customer>.Failure(
                    "Customer profile not found.");
            }

            var customerDto =
                _mapper.Map<Customer>(customer);

            return Result<Customer>.Ok(
                customer);
        }

        public async Task CreateFromRegistrationAsync(CustomerRegisteredEvent customerEvent)
        {
            var existingCustomer = await _repository.GetByUserIdAsync(customerEvent.UserId);

            if (existingCustomer != null)
            {
                return;
            }

            var customer = new Customer
            {
                UserId = customerEvent.UserId,
                FirstName = customerEvent.FirstName,
                LastName = customerEvent.LastName,
                Email = customerEvent.Email,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "IdentityService",
                CustomerNumber = CustomerNumberGenerator.Generate()
            };

            await _repository.CreateAsync(customer);
        }

        public async Task UpdateUserIdAsync(int customerId,int userId)
        {
            await _repository.UpdateUserIdAsync(customerId,userId);
        }
    }
}

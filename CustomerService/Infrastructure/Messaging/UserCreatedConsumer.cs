using CustomerService.Application.Features.Customers.Interfaces;
using Shared.Contracts.Events;
using System.Text.Json;

namespace CustomerService.Infrastructure.Messaging;

public class UserCreatedConsumer
{
    private readonly ICustomerService _customerService;
    public UserCreatedConsumer(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task HandleAsync(string message)
    {
        var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);

        if (userEvent == null)
        {
            throw new InvalidOperationException(
                "Invalid UserCreatedEvent message.");
        }

        await _customerService.UpdateUserIdAsync( userEvent.CustomerId,userEvent.UserId);
    }
}
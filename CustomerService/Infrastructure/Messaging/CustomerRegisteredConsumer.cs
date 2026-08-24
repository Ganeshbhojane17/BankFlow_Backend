using CustomerService.Application.Features.Customers.Interfaces;
using System.Text.Json;

namespace CustomerService.Infrastructure.Messaging;

public class CustomerRegisteredConsumer
{
    private readonly ICustomerService _customerService;

    public CustomerRegisteredConsumer(
        ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task HandleAsync(string message)
    {
        var customerEvent =
            JsonSerializer.Deserialize<CustomerRegisteredEvent>(message);

        if (customerEvent == null)
        {
            throw new InvalidOperationException(
                "Invalid CustomerRegisteredEvent message.");
        }

        await _customerService
            .CreateFromRegistrationAsync(customerEvent);
    }
}
using IdentityService.Features.Auth.Interfaces;
using Shared.Contracts.Events;
using System.Text.Json;

namespace IdentityService.Infrastructure.Messaging;

public class UserProvisioningConsumer
{
    private readonly IAuthService _authService;

    public UserProvisioningConsumer(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task HandleAsync(string message)
    {
        var userEvent = JsonSerializer.Deserialize<UserProvisioningRequested>(message);
        if (userEvent == null)
        {
            throw new InvalidOperationException("Invalid UserProvisioningRequested event.");
        }
        await _authService.CreateFromCustomerAsync(userEvent);
    }
}
using IdentityService.Features.Auth.Interfaces;
using IdentityService.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Messaging;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMessagesAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while processing outbox messages.");
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Outbox processor is stopping.");
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();

        var messages = await outboxRepository.GetPendingMessagesAsync(20);

        foreach (var message in messages)
        {
            try
            {
                await rabbitMqPublisher.PublishAsync(message.Payload, message.RoutingKey);

                await outboxRepository.MarkAsProcessedAsync(message.Id);

                _logger.LogInformation( "Outbox message {MessageId} published successfully.", message.Id);
            }
            catch (Exception ex)
            {
                await outboxRepository.MarkAsFailedAsync( message.Id, ex.Message);

                _logger.LogError( ex,"Failed to publish outbox message {MessageId}.", message.Id);
            }
        }
    }
}
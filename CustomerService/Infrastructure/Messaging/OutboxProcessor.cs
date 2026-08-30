using CustomerService.Application.Common.Interfaces;

namespace CustomerService.Infrastructure.Messaging;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                _logger.LogError(ex, "Error while processing CustomerService outbox.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
        var messages = await outboxRepository.GetPendingMessagesAsync(20);
        foreach (var message in messages)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await publisher.PublishAsync(message.Payload, message.RoutingKey);
                await outboxRepository.MarkAsProcessedAsync(message.Id);
                _logger.LogInformation( "Outbox message {MessageId} published.", message.Id);
            }
            catch (Exception ex)
            {
                await outboxRepository.MarkAsFailedAsync(message.Id, ex.Message);
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}.", message.Id);
            }
        }
    }
}
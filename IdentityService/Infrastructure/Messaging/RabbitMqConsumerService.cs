using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace IdentityService.Infrastructure.Messaging;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    private const string QueueName = "user-provisioning-queue";
    private const string RoutingKey = "user.provisioning.requested";

    public RabbitMqConsumerService(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            _connection =
                await factory.CreateConnectionAsync();

            _channel =
                await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: _options.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken);

            _logger.LogInformation(
                "IdentityService RabbitMQ consumer started. Queue: {Queue}",
                QueueName);

            var consumer =
                new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                await ProcessMessageAsync(
                    args,
                    stoppingToken);
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "RabbitMQ consumer is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "IdentityService RabbitMQ consumer failed.");
        }
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        try
        {
            var message =
                Encoding.UTF8.GetString(args.Body.ToArray());

            _logger.LogInformation(
                "Received RabbitMQ message: {Message}",
                message);

            using var scope =
                _scopeFactory.CreateScope();

            var consumer =
                scope.ServiceProvider
                    .GetRequiredService<UserProvisioningConsumer>();

            await consumer.HandleAsync(message);

            await _channel!.BasicAckAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "User provisioning completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing user provisioning message.");

            await _channel!.BasicNackAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken: cancellationToken);
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(
                cancellationToken);
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(
                cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
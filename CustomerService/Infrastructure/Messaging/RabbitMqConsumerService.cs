using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerService.Infrastructure.Messaging;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerService(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await ConfigureRabbitMqAsync(stoppingToken);
        await StartConsumerAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ConfigureRabbitMqAsync(CancellationToken cancellationToken)
    {
        await _channel!.ExchangeDeclareAsync(
            RabbitMqConstants.MainExchange,
            ExchangeType.Direct,
            true,
            false,
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            RabbitMqConstants.RetryExchange,
            ExchangeType.Direct,
            true,
            false,
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            RabbitMqConstants.DeadLetterExchange,
            ExchangeType.Direct,
            true,
            false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.CustomerRegisteredQueue,
            true,
            false,
            false,
            cancellationToken: cancellationToken);

        var retryArguments =
            new Dictionary<string, object?>
            {
                ["x-message-ttl"] =
                    _options.RetryDelaySeconds * 1000,

                ["x-dead-letter-exchange"] =
                    RabbitMqConstants.MainExchange,

                ["x-dead-letter-routing-key"] =
                    RabbitMqConstants.CustomerRegisteredRoutingKey
            };

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.CustomerRegisteredRetryQueue,
            true,
            false,
            false,
            retryArguments,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.CustomerRegisteredDlq,
            true,
            false,
            false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            RabbitMqConstants.CustomerRegisteredQueue,
            RabbitMqConstants.MainExchange,
            RabbitMqConstants.CustomerRegisteredRoutingKey,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            RabbitMqConstants.CustomerRegisteredRetryQueue,
            RabbitMqConstants.RetryExchange,
            RabbitMqConstants.CustomerRegisteredRetryRoutingKey,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            RabbitMqConstants.CustomerRegisteredDlq,
            RabbitMqConstants.DeadLetterExchange,
            RabbitMqConstants.CustomerRegisteredDlqRoutingKey,
            cancellationToken: cancellationToken);
    }

    private async Task StartConsumerAsync(
        CancellationToken stoppingToken)
    {
        var consumer =
            new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            await ProcessMessageAsync(
                args,
                stoppingToken);
        };

        await _channel!.BasicConsumeAsync(
            RabbitMqConstants.CustomerRegisteredQueue,
            false,
            consumerTag: "",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        try
        {
            var body =
                Encoding.UTF8.GetString(
                    args.Body.ToArray());

            using var scope =
                _scopeFactory.CreateScope();

            var handler =
                scope.ServiceProvider
                    .GetRequiredService<CustomerRegisteredConsumer>();

            await handler.HandleAsync(body);

            await _channel!.BasicAckAsync(
                args.DeliveryTag,
                false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"RabbitMQ processing failed: {ex.Message}");

            await HandleFailureAsync(
                args,
                cancellationToken);
        }
    }

    private async Task HandleFailureAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        var retryCount =
            GetRetryCount(
                args.BasicProperties);

        if (retryCount >= _options.MaxRetryCount)
        {
            await MoveToDeadLetterQueueAsync(
                args,
                cancellationToken);

            return;
        }

        await PublishToRetryQueueAsync(
            args,
            retryCount + 1,
            cancellationToken);

        await _channel!.BasicAckAsync(
            args.DeliveryTag,
            false);
    }

    private int GetRetryCount(
        IReadOnlyBasicProperties properties)
    {
        if (properties.Headers == null)
            return 0;

        if (!properties.Headers.TryGetValue(
                "x-retry-count",
                out var value))
        {
            return 0;
        }

        if (value is byte[] bytes &&
            int.TryParse(
                Encoding.UTF8.GetString(bytes),
                out var count))
        {
            return count;
        }

        if (value is int intValue)
            return intValue;

        return 0;
    }

    private async Task PublishToRetryQueueAsync(BasicDeliverEventArgs args, int retryCount,
    CancellationToken cancellationToken)
    {
        var properties = new BasicProperties
        {
            ContentType = args.BasicProperties.ContentType,
            DeliveryMode = DeliveryModes.Persistent,
            Headers =
                new Dictionary<string, object?>
                {
                    ["x-retry-count"] = retryCount
                }
        };

        await _channel!.BasicPublishAsync(exchange: RabbitMqConstants.RetryExchange,
            routingKey: RabbitMqConstants.CustomerRegisteredRetryRoutingKey,
            mandatory: false, basicProperties: properties,
            body: args.Body.ToArray(), cancellationToken: cancellationToken);
    }

    private async Task MoveToDeadLetterQueueAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
    {
        var properties = new BasicProperties
        {
            ContentType = args.BasicProperties.ContentType,
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel!.BasicPublishAsync(exchange:RabbitMqConstants.DeadLetterExchange,
            routingKey: RabbitMqConstants.CustomerRegisteredDlqRoutingKey, mandatory: false, basicProperties: properties,
            body: args.Body.ToArray(), cancellationToken: cancellationToken);

        await _channel.BasicAckAsync(
            args.DeliveryTag,
            multiple: false);

        Console.WriteLine(
            "Message moved to Dead Letter Queue.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
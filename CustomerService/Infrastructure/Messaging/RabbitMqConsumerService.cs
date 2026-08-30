using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerService.Infrastructure.Messaging;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

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
                await factory.CreateConnectionAsync(
                    stoppingToken);

            _channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            await ConfigureRabbitMqAsync(
                stoppingToken);

            await StartConsumerAsync(
                stoppingToken);

            _logger.LogInformation(
                "CustomerService RabbitMQ consumer started.");

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
                "RabbitMQ consumer stopped because of an error.");
        }
    }


    // =====================================================
    // RabbitMQ Configuration
    // =====================================================

    private async Task ConfigureRabbitMqAsync(
        CancellationToken cancellationToken)
    {
        // -------------------------------------------------
        // Exchanges
        // -------------------------------------------------

        await _channel!.ExchangeDeclareAsync(
            RabbitMqConstants.MainExchange,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            RabbitMqConstants.RetryExchange,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            RabbitMqConstants.DeadLetterExchange,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);


        // -------------------------------------------------
        // Customer Registered
        // -------------------------------------------------

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.CustomerRegisteredQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var customerRetryArguments =
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
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: customerRetryArguments,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.CustomerRegisteredDlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);


        // -------------------------------------------------
        // User Created
        // -------------------------------------------------

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.UserCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var userCreatedRetryArguments =
            new Dictionary<string, object?>
            {
                ["x-message-ttl"] =
                    _options.RetryDelaySeconds * 1000,

                ["x-dead-letter-exchange"] =
                    RabbitMqConstants.MainExchange,

                ["x-dead-letter-routing-key"] =
                    RabbitMqConstants.UserCreatedRoutingKey
            };

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.UserCreatedRetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: userCreatedRetryArguments,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            RabbitMqConstants.UserCreatedDlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);


        // -------------------------------------------------
        // Customer Registered Bindings
        // -------------------------------------------------

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


        // -------------------------------------------------
        // User Created Bindings
        // -------------------------------------------------

        await _channel.QueueBindAsync(
            RabbitMqConstants.UserCreatedQueue,
            RabbitMqConstants.MainExchange,
            RabbitMqConstants.UserCreatedRoutingKey,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            RabbitMqConstants.UserCreatedRetryQueue,
            RabbitMqConstants.RetryExchange,
            RabbitMqConstants.UserCreatedRetryRoutingKey,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            RabbitMqConstants.UserCreatedDlq,
            RabbitMqConstants.DeadLetterExchange,
            RabbitMqConstants.UserCreatedDlqRoutingKey,
            cancellationToken: cancellationToken);
    }


    // =====================================================
    // Start Consumers
    // =====================================================

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


        // Customer Registered

        await _channel!.BasicConsumeAsync(
            queue:
                RabbitMqConstants.CustomerRegisteredQueue,

            autoAck: false,

            consumerTag: "",

            noLocal: false,

            exclusive: false,

            arguments: null,

            consumer: consumer,

            cancellationToken: stoppingToken);


        // User Created

        await _channel.BasicConsumeAsync(
            queue:
                RabbitMqConstants.UserCreatedQueue,

            autoAck: false,

            consumerTag: "",

            noLocal: false,

            exclusive: false,

            arguments: null,

            consumer: consumer,

            cancellationToken: stoppingToken);
    }


    // =====================================================
    // Process Message
    // =====================================================

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        try
        {
            var body =
                Encoding.UTF8.GetString(
                    args.Body.ToArray());

            _logger.LogInformation(
                "RabbitMQ message received. RoutingKey: {RoutingKey}",
                args.RoutingKey);


            using var scope =
                _scopeFactory.CreateScope();


            // ---------------------------------------------
            // customer.registered
            // ---------------------------------------------

            if (args.RoutingKey ==
                RabbitMqConstants.CustomerRegisteredRoutingKey)
            {
                var handler =
                    scope.ServiceProvider
                        .GetRequiredService<
                            CustomerRegisteredConsumer>();

                await handler.HandleAsync(body);
            }


            // ---------------------------------------------
            // user.created
            // ---------------------------------------------

            else if (args.RoutingKey ==
                     RabbitMqConstants.UserCreatedRoutingKey)
            {
                var handler =
                    scope.ServiceProvider
                        .GetRequiredService<
                            UserCreatedConsumer>();

                await handler.HandleAsync(body);
            }


            // ---------------------------------------------
            // Unknown message
            // ---------------------------------------------

            else
            {
                throw new InvalidOperationException(
                    $"Unknown routing key: {args.RoutingKey}");
            }


            // ---------------------------------------------
            // ACK
            // ---------------------------------------------

            await _channel!.BasicAckAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "RabbitMQ message processed successfully. RoutingKey: {RoutingKey}",
                args.RoutingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "RabbitMQ message processing failed. RoutingKey: {RoutingKey}",
                args.RoutingKey);

            await HandleFailureAsync(
                args,
                cancellationToken);
        }
    }


    // =====================================================
    // Failure Handling
    // =====================================================

    private async Task HandleFailureAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        var retryCount =
            GetRetryCount(
                args.BasicProperties);


        // ---------------------------------------------
        // Maximum retries reached
        // ---------------------------------------------

        if (retryCount >= _options.MaxRetryCount)
        {
            await MoveToDeadLetterQueueAsync(
                args,
                cancellationToken);

            return;
        }


        // ---------------------------------------------
        // Send to retry queue
        // ---------------------------------------------

        await PublishToRetryQueueAsync(
            args,
            retryCount + 1,
            cancellationToken);


        // Original message successfully transferred
        // to retry queue.

        await _channel!.BasicAckAsync(
            deliveryTag: args.DeliveryTag,
            multiple: false,
            cancellationToken: cancellationToken);


        _logger.LogWarning(
            "Message moved to retry queue. RoutingKey: {RoutingKey}, RetryCount: {RetryCount}",
            args.RoutingKey,
            retryCount + 1);
    }


    // =====================================================
    // Get Retry Count
    // =====================================================

    private int GetRetryCount(
        IReadOnlyBasicProperties properties)
    {
        if (properties.Headers == null)
        {
            return 0;
        }

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
        {
            return intValue;
        }

        return 0;
    }


    // =====================================================
    // Publish to Retry Queue
    // =====================================================

    private async Task PublishToRetryQueueAsync(
        BasicDeliverEventArgs args,
        int retryCount,
        CancellationToken cancellationToken)
    {
        string retryRoutingKey;

        if (args.RoutingKey ==
            RabbitMqConstants.UserCreatedRoutingKey)
        {
            retryRoutingKey =
                RabbitMqConstants.UserCreatedRetryRoutingKey;
        }
        else
        {
            retryRoutingKey =
                RabbitMqConstants.CustomerRegisteredRetryRoutingKey;
        }


        var properties =
            new BasicProperties
            {
                ContentType =
                    args.BasicProperties.ContentType,

                DeliveryMode =
                    DeliveryModes.Persistent,

                Headers =
                    new Dictionary<string, object?>
                    {
                        ["x-retry-count"] =
                            retryCount
                    }
            };


        await _channel!.BasicPublishAsync(
            exchange:
                RabbitMqConstants.RetryExchange,

            routingKey:
                retryRoutingKey,

            mandatory: false,

            basicProperties:
                properties,

            body:
                args.Body.ToArray(),

            cancellationToken:
                cancellationToken);
    }


    // =====================================================
    // Dead Letter Queue
    // =====================================================

    private async Task MoveToDeadLetterQueueAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        string dlqRoutingKey;

        if (args.RoutingKey ==
            RabbitMqConstants.UserCreatedRoutingKey)
        {
            dlqRoutingKey =
                RabbitMqConstants.UserCreatedDlqRoutingKey;
        }
        else
        {
            dlqRoutingKey =
                RabbitMqConstants.CustomerRegisteredDlqRoutingKey;
        }


        var properties =
            new BasicProperties
            {
                ContentType =
                    args.BasicProperties.ContentType,

                DeliveryMode =
                    DeliveryModes.Persistent
            };


        await _channel!.BasicPublishAsync(
            exchange:
                RabbitMqConstants.DeadLetterExchange,

            routingKey:
                dlqRoutingKey,

            mandatory: false,

            basicProperties:
                properties,

            body:
                args.Body.ToArray(),

            cancellationToken:
                cancellationToken);


        await _channel.BasicAckAsync(
            deliveryTag: args.DeliveryTag,
            multiple: false,
            cancellationToken: cancellationToken);


        _logger.LogError(
            "Message moved to Dead Letter Queue. RoutingKey: {RoutingKey}",
            args.RoutingKey);
    }


    // =====================================================
    // Stop
    // =====================================================

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        try
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
        }
        finally
        {
            await base.StopAsync(
                cancellationToken);
        }
    }
}
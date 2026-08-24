using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace IdentityService.Infrastructure.Messaging;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly RabbitMqOptions _options;

    public RabbitMqPublisher(
        IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task PublishAsync(
        string message,
        string routingKey)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        var body =
            Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}
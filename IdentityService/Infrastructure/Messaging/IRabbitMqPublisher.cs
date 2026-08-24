namespace IdentityService.Infrastructure.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(string message, string routingKey);
    }
}

namespace CustomerService.Infrastructure.Messaging
{
    public class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";


        public string ExchangeName { get; set; } =
            RabbitMqConstants.MainExchange;

        public string QueueName { get; set; } =
            RabbitMqConstants.CustomerRegisteredQueue;

        public string RoutingKey { get; set; } =
            RabbitMqConstants.CustomerRegisteredRoutingKey;


        public int RetryDelaySeconds { get; set; } = 10;

        public int MaxRetryCount { get; set; } = 3;
    }
}

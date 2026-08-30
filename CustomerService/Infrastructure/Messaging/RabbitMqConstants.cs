namespace CustomerService.Infrastructure.Messaging
{
    public static class RabbitMqConstants
    {
        public const string MainExchange = "bankflow.events";
        public const string RetryExchange = "bankflow.retry.exchange";
        public const string DeadLetterExchange = "bankflow.dlx";
        // Customer Registered
        public const string CustomerRegisteredQueue = "customer.registered.queue";
        public const string CustomerRegisteredRetryQueue = "customer.registered.retry";
        public const string CustomerRegisteredDlq = "customer.registered.dlq";
        public const string CustomerRegisteredRoutingKey = "customer.registered";
        public const string CustomerRegisteredRetryRoutingKey = "customer.registered.retry";
        public const string CustomerRegisteredDlqRoutingKey = "customer.registered.dlq";

        // User Created

        public const string UserCreatedQueue =
            "user-created-queue";

        public const string UserCreatedRoutingKey =
            "user.created";

        public const string UserCreatedRetryQueue =
            "user-created-retry-queue";

        public const string UserCreatedRetryRoutingKey =
            "user.created.retry";

        public const string UserCreatedDlq =
            "user-created-dlq";

        public const string UserCreatedDlqRoutingKey =
            "user.created.dlq";
    }
}

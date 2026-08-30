namespace CustomerService.Domain.Entities
{
    public class OutboxMessage
    {
        public long Id { get; set; }

        public Guid EventId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string RoutingKey { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public DateTime? ProcessedOn { get; set; }

        public int RetryCount { get; set; }

        public string? ErrorMessage { get; set; }
    }
}

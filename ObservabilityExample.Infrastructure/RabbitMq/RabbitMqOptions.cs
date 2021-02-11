using RawRabbit.Configuration.Exchange;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public class RabbitMqOptions
    {
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public ushort PrefetchCount { get; set; }
    }
}
using System;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public interface ICorrelationContext
    {
        Guid Id { get; }
        Guid ResourceId { get; }
        string TraceId { get; }
        string SpanContext { get; }
        string ConnectionId { get; }
        string Name { get; }
        string Origin { get; }
        string Resource { get; }
        DateTime CreatedAt { get; }
        int Retries { get; set; }
    }
    
}
using System;
using Newtonsoft.Json;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public class CorrelationContext : ICorrelationContext
    {
        public Guid Id { get; }
        public Guid ResourceId { get; }
        public string TraceId { get; }
        public string SpanContext { get; }
        public string ConnectionId { get; }
        public string Name { get; }
        public string Origin { get; }
        public string Resource { get; }
        public DateTime CreatedAt { get; }
        public int Retries { get; set; }

        public CorrelationContext() { }

        private CorrelationContext(Guid id)
        {
            Id = id;
        }

        [JsonConstructor]
        private CorrelationContext(Guid id, Guid resourceId,
                                   string traceId, string spanContext,
                                   string connectionId, string executionId, string name,
                                   string origin, string resource, int retries)
        {
            Id = id;
            ResourceId = resourceId;
            TraceId = traceId;
            SpanContext = spanContext;
            ConnectionId = connectionId;
            Name = string.IsNullOrWhiteSpace(name) ? string.Empty : GetName(name);
            Origin = string.IsNullOrWhiteSpace(origin)
                             ? string.Empty
                             : origin.StartsWith("/")
                                     ? origin.Remove(0, 1)
                                     : origin;
            Resource = resource;
            Retries = retries;
            CreatedAt = DateTime.UtcNow;
        }

        public static ICorrelationContext Empty
            => new CorrelationContext();

        public static ICorrelationContext FromId(Guid id)
            => new CorrelationContext(id);

        public static ICorrelationContext From<T>(ICorrelationContext context)
            => Create<T>(context.Id, context.ResourceId, context.TraceId,
                    context.ConnectionId,
                    context.Origin, context.Resource);

        public static ICorrelationContext Create<T>(Guid id, Guid resourceId, string origin,
                                                    string traceId, string spanContext, string connectionId,
                                                    string resource = "")
        
            => new CorrelationContext(id, resourceId, traceId,
                    spanContext, connectionId, string.Empty,
                    typeof(T).Name, origin,
                    resource, 0);

        private static string GetName(string name)
            => name.Underscore().ToLowerInvariant();
    }
}
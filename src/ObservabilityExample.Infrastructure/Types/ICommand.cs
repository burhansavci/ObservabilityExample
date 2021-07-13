using MediatR;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;

namespace ObservabilityExample.Infrastructure.Types
{
    public interface ICommand
    {
        [JsonIgnore]
        ICorrelationContext CorrelationContext { get; set; }
    }
}
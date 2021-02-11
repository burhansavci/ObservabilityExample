using System.Threading.Tasks;
using MediatR;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public interface IBusPublisher
    {
        Task PublishAsync<TRequest>(TRequest request, RabbitMqOptions options, ICorrelationContext context) where TRequest : IRequest;
    }
}
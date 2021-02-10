using System.Threading.Tasks;
using MediatR;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public interface IBusPublisher
    {
        Task PublishAsync<TRequest>(TRequest request, ICorrelationContext context) where TRequest : IRequest;
    }
}
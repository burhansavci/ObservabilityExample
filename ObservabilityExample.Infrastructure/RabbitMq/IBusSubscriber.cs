using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ObservabilityExample.Infrastructure.Types;
using RawRabbit.Configuration.Exchange;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public interface IBusSubscriber
    {
        Task SubscribeAsync<TRequest>(RabbitMqOptions options, CancellationToken ct = default) where TRequest : IRequest, ICommand;
    }
}
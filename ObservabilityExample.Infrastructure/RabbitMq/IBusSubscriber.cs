using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ObservabilityExample.Infrastructure.Types;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public interface IBusSubscriber
    {
        public Task SubscribeAsync<TCommand>(RabbitMqOptions options, CancellationToken ct = default)
                where TCommand : IRequest, ICommand;
    }
}
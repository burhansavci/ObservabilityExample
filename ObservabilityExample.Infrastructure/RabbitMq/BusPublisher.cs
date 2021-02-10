using System.Threading.Tasks;
using MediatR;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public class BusPublisher : IBusPublisher
    {
        private readonly IBusClient busClient;

        public BusPublisher(IBusClient busClient)
        {
            this.busClient = busClient;
        }
        
        public async Task PublishAsync<TRequest>(TRequest request, ICorrelationContext context)
                where TRequest : IRequest =>
                await busClient.PublishAsync(request, ctx => ctx.UseMessageContext(context));
    }
}
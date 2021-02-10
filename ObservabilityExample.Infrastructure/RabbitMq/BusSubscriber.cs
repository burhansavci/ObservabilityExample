using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObservabilityExample.Infrastructure.Types;
using RawRabbit;
using RawRabbit.Configuration.Exchange;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public class BusSubscriber : IBusSubscriber
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IBusClient busClient;
        private readonly IMediator mediator;
        private readonly ILogger<BusSubscriber> logger;


        public BusSubscriber(IApplicationBuilder app)
        {
            logger = app.ApplicationServices.GetService<ILogger<BusSubscriber>>();
            serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            busClient = serviceProvider.GetService<IBusClient>();
            mediator = serviceProvider.GetService<IMediator>();
        }

        public Task SubscribeAsync<TCommand>(RabbitMqOptions options, CancellationToken ct = default) where TCommand : IRequest, ICommand
        {
            return busClient.SubscribeAsync<TCommand>(async msg =>
                    {
                        var messageName = msg.GetType().Name;
                        logger.LogInformation($"Handling a message: '{messageName}' with correlation id: '{msg.CorrelationContext.Id}'.");

                        await mediator.Send(msg, ct);

                        logger.LogInformation($"Handled a message: '{messageName}' with correlation id: '{msg.CorrelationContext.Id}'.");
                    },
                    ctx =>
                            ctx.UseSubscribeConfiguration(cfg =>
                            {
                                cfg.Consume(c => c.WithPrefetchCount(options.PrefetchCount));
                                if (options.ExchangeName != default)
                                    cfg.OnDeclaredExchange(e => e.WithName(options.ExchangeName));
                                if (options.QueueName != default)
                                    cfg.FromDeclaredQueue(q => q.WithName(options.QueueName));
                                if (options.RoutingKey != default)
                                    cfg.Consume(c => c.WithRoutingKey(options.RoutingKey));
                                if (options.ExchangeType != default)
                                    cfg.OnDeclaredExchange(e => e.WithType(options.ExchangeType));
                            }), ct);
        }
    }
}
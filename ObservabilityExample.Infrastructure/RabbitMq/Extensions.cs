using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;

namespace ObservabilityExample.Infrastructure.RabbitMq
{
    public static class Extensions
    {
        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
            => new BusSubscriber(app);
        
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new RawRabbitConfiguration();
            configuration.GetSection("RabbitMq").Bind(options);

            services.AddRawRabbit(new RawRabbitOptions
            {
                    ClientConfiguration = options,
                    Plugins = p => p
                                  .UseMessageContext<CorrelationContext>()
                                  .UseContextForwarding(),

            }).AddSingleton<IBusPublisher, BusPublisher>();
            
            return services;
        }
    }
}
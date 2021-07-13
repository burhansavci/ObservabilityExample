using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;

namespace ObservabilityExample.Infrastructure.Jaeger
{
    public class JaegerSubscriberStagedMiddleware : StagedMiddleware
    {
        public override string StageMarker =>  RawRabbit.Pipe.StageMarker.MessageDeserialized;

        private static readonly ActivitySource ActivitySource = new("JaegerStagedMiddleware");
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();
        
        private readonly ILogger<JaegerSubscriberStagedMiddleware> logger;

        public JaegerSubscriberStagedMiddleware(ILogger<JaegerSubscriberStagedMiddleware> logger) => this.logger = logger;
        
        public override async Task InvokeAsync(IPipeContext context, CancellationToken token = new CancellationToken())
        {
            
            var parentContext = Propagator.Extract(default, context.GetDeliveryEventArgs().BasicProperties, ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;
            
            var activityName = $"{context.GetDeliveryEventArgs().RoutingKey} receive";
            using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);
            try
            {
                logger.LogInformation($"Message received: [{context.GetMessageType().Name}]");
            
                activity?.SetTag("message", JsonConvert.SerializeObject(context.GetMessage()));
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination_kind", "queue");
                activity?.SetTag("messaging.destination", context.GetExchangeDeclaration().Name);
                activity?.SetTag("messaging.rabbitmq.routing_key",context.GetDeliveryEventArgs().RoutingKey);
                
                await Next.InvokeAsync(context, token);
            }
            catch (Exception ex)
            {
                activity?.SetTag("error", true);
                logger.LogError(ex, "Message processing failed.");
            }
        }
        
        private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
        {
            try
            {
                if (props.Headers.TryGetValue(key, out var value))
                {
                    var bytes = value as byte[];
                    return new[] {Encoding.UTF8.GetString(bytes ?? Array.Empty<byte>())};
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to extract trace context: {ex}");
            }

            return Enumerable.Empty<string>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;

namespace ObservabilityExample.Infrastructure.Jaeger
{
    public class JaegerPublisherStagedMiddleware : StagedMiddleware
    {
        public override string StageMarker => RawRabbit.Operations.Publish.PublishStage.PreMessagePublish.ToString();

        private static readonly ActivitySource ActivitySource = new("JaegerStagedMiddleware");
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

        private readonly ILogger<JaegerPublisherStagedMiddleware> logger;

        public JaegerPublisherStagedMiddleware(ILogger<JaegerPublisherStagedMiddleware> logger) => this.logger = logger;


        public override async Task InvokeAsync(IPipeContext context, CancellationToken token = new CancellationToken())
        {
            try
            {
                var activityName = $"{context.GetBasicPublishConfiguration().RoutingKey} send";
                
                using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Producer);

                // Depending on Sampling (and whether a listener is registered or not), the
                // activity above may not be created.
                // If it is created, then propagate its context.
                // If it is not created, the propagate the Current context,
                // if any.
                ActivityContext contextToInject = default;
                if (activity != null)
                {
                    contextToInject = activity.Context;
                }
                else if (Activity.Current != null)
                {
                    contextToInject = Activity.Current.Context;
                }
                
                Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), context.GetBasicPublishConfiguration().BasicProperties,
                        InjectTraceContextIntoBasicProperties);
                
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination_kind", "queue");
                activity?.SetTag("messaging.destination", context.GetBasicPublishConfiguration().ExchangeName);
                activity?.SetTag("messaging.rabbitmq.routing_key", context.GetBasicPublishConfiguration().RoutingKey);
                var body = $"Published message: DateTime.Now = {DateTime.Now}.";

                await Next.InvokeAsync(context, token);

                logger.LogInformation($"Message sent: [{body}]");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Message publishing failed.");
                throw;
            }
        }

        private void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
        {
            try
            {
                props.Headers ??= new Dictionary<string, object>();

                props.Headers[key] = value;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to inject trace context.");
            }
        }
    }
}
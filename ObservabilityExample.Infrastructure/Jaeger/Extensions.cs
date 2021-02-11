using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObservabilityExample.Services.Products;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ObservabilityExample.Infrastructure.Jaeger
{
    public static class Extensions
    {
        public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration)
        {
            var jaegerOptions = new JaegerOptions();
            configuration.GetSection("Jaeger").Bind(jaegerOptions);

            services.AddOpenTelemetryTracing(config => config
                                                      .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                                                                         .AddService(jaegerOptions.ServiceName))
                                                      .SetSampler(GetSampler(jaegerOptions))
                                                      .AddJaegerExporter(c =>
                                                       {
                                                           c.AgentHost = jaegerOptions.Host;
                                                           c.AgentPort = jaegerOptions.Port;
                                                           c.MaxPayloadSizeInBytes = jaegerOptions.MaxPayloadSizeInBytes;
                                                       })
                                                      .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                                                      .AddAspNetCoreInstrumentation(x =>
                                                               x.Filter = context =>
                                                                       !jaegerOptions.ExcludePaths.Contains(context.Request.Path.Value,
                                                                               StringComparer.OrdinalIgnoreCase))
                                                      .AddHttpClientInstrumentation());

            return services;
        }

        private static Sampler GetSampler(JaegerOptions jaegerOptions)
        {
            return jaegerOptions.Sampler switch
                   {
                           "alwaysOn" => new AlwaysOnSampler(),
                           "alwaysOff" => new AlwaysOffSampler(),
                           "probabilistic" => new TraceIdRatioBasedSampler(jaegerOptions.SamplingRate),
                           _ => new AlwaysOnSampler()
                   };
        }
    }
}
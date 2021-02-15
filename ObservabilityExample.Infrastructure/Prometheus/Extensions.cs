using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObservabilityExample.Infrastructure.Prometheus.Internals;
using Prometheus;
using Prometheus.SystemMetrics;

namespace ObservabilityExample.Infrastructure.Prometheus
{
    public static class Extensions
    {
        public static IServiceCollection AddPrometheus(this IServiceCollection services, IConfiguration configuration)
        {
            var prometheusOptions = configuration.GetOptions<PrometheusOptions>("Prometheus");
            services.AddSingleton(prometheusOptions);

            if (!prometheusOptions.Enabled)
                return services;

            services.AddHostedService<PrometheusJob>();
            services.AddSingleton<PrometheusMiddleware>();
            services.AddSystemMetrics();

            return services;
        }

        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<PrometheusOptions>();
            if (!options.Enabled)
                return app;

            var endpoint = string.IsNullOrWhiteSpace(options.Endpoint)
                                   ? "/metrics"
                                   : options.Endpoint.StartsWith("/")
                                           ? options.Endpoint
                                           : $"/{options.Endpoint}";

            return app
                  .UseMiddleware<PrometheusMiddleware>()
                  .UseHttpMetrics()
                  .UseMetricServer(endpoint);
        }
    }
}
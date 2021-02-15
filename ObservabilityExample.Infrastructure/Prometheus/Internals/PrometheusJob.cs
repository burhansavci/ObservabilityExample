using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus.DotNetRuntime;

namespace ObservabilityExample.Infrastructure.Prometheus.Internals
{
    internal sealed class PrometheusJob : IHostedService
    {
        private IDisposable collector;
        private readonly bool enabled;

        public PrometheusJob(PrometheusOptions options, ILogger<PrometheusJob> logger)
        {
            enabled = options.Enabled;
            logger.LogInformation($"Prometheus integration is {(enabled ? "enabled" : "disabled")}.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (enabled)
            {
                collector = DotNetRuntimeStatsBuilder
                           .Customize()
                           .WithContentionStats()
                           .WithJitStats()
                           .WithThreadPoolSchedulingStats()
                           .WithThreadPoolStats()
                           .WithGcStats()
                           .WithExceptionStats()
                           .StartCollecting();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            collector?.Dispose();

            return Task.CompletedTask;
        }
    }
}
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Json;

namespace ObservabilityExample.Infrastructure.Logging
{
    public static class Extensions
    {
        public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, string applicationName = null)
            => hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var appOptions = context.Configuration.GetOptions<AppOptions>("App");
                var seqOptions = context.Configuration.GetOptions<SeqOptions>("Seq");
                var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("Serilog");
                var fileOptions = context.Configuration.GetOptions<FileOptions>("File");
                var fluentdOptions = context.Configuration.GetOptions<FluentdOptions>("Fluentd");

                if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out var level))
                    level = LogEventLevel.Information;

                applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;
                loggerConfiguration.Enrich.FromLogContext()
                                   .MinimumLevel.Is(level)
                                   .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                                   .Enrich.WithProperty("ApplicationName", applicationName);
                serilogOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
                                                                                      .ByExcluding(Matching.WithProperty<string>("RequestPath",
                                                                                               n => n.EndsWith(p))));
                Configure(loggerConfiguration, seqOptions, serilogOptions,
                        fileOptions, fluentdOptions);
                
            }).ConfigureLogging((_, config) => { config.ClearProviders(); });

        private static void Configure(LoggerConfiguration loggerConfiguration, SeqOptions seqOptions, SerilogOptions serilogOptions,
                                      FileOptions fileOptions, FluentdOptions fluentdOptions)
        {
            if (seqOptions.Enabled)
                loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);

            if (serilogOptions.ConsoleEnabled)
                loggerConfiguration.WriteTo.Console();

            if (fluentdOptions.Enabled)
                loggerConfiguration.WriteTo.Fluentd(fluentdOptions.Host, fluentdOptions.Port, fluentdOptions.Tag);

            if (fileOptions.Enabled)
            {
                var path = string.IsNullOrWhiteSpace(fileOptions.Path) ? "logs/logs.log" : fileOptions.Path;

                if (!Enum.TryParse<RollingInterval>(fileOptions.Interval, true, out var interval))
                    interval = RollingInterval.Day;

                loggerConfiguration.WriteTo.File(new JsonFormatter(), path, rollingInterval: interval);
            }
        }
    }
}
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

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
                if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out var level))
                {
                    level = LogEventLevel.Information;
                }

                applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;
                loggerConfiguration.Enrich.FromLogContext()
                                   .MinimumLevel.Is(level)
                                   .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                                   .Enrich.WithProperty("ApplicationName", applicationName);

                serilogOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
                                                                                      .ByExcluding(Matching.WithProperty<string>("RequestPath",
                                                                                               n => n.EndsWith(p))));
                Configure(loggerConfiguration, seqOptions, serilogOptions);
            });

        private static void Configure(LoggerConfiguration loggerConfiguration, SeqOptions seqOptions, SerilogOptions serilogOptions)
        {
            if (seqOptions.Enabled)
                loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);

            if (serilogOptions.ConsoleEnabled)
                loggerConfiguration.WriteTo.Console();
        }
    }
}
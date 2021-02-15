using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ObservabilityExample.Infrastructure.Prometheus.Internals
{
    internal sealed class PrometheusMiddleware : IMiddleware
    {
        private readonly ISet<string> allowedHosts;
        private readonly string endpoint;
        private readonly string apiKey;

        public PrometheusMiddleware(PrometheusOptions options)
        {
            allowedHosts = new HashSet<string>(options.AllowedHosts ?? Array.Empty<string>());
            endpoint = string.IsNullOrWhiteSpace(options.Endpoint)
                               ? "/metrics"
                               : options.Endpoint.StartsWith("/")
                                       ? options.Endpoint
                                       : $"/{options.Endpoint}";
            apiKey = options.ApiKey;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            if (context.Request.Path != endpoint)
                return next(context);

            if (string.IsNullOrWhiteSpace(apiKey))
                return next(context);

            if (request.Query.TryGetValue("apiKey", out var key) && key == apiKey)
                return next(context);
            
            var host = context.Request.Host.Host;
            if (allowedHosts.Contains(host))
                return next(context);


            if (!request.Headers.TryGetValue("x-forwarded-for", out var forwardedFor))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            if (allowedHosts.Contains(forwardedFor))
                return next(context);

            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }
    }
}
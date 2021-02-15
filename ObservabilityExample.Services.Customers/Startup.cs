using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ObservabilityExample.Infrastructure.Jaeger;
using ObservabilityExample.Infrastructure.Prometheus;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Services.Customers.Commands;
using ObservabilityExample.Services.Customers.Domain;
using ObservabilityExample.Services.Customers.Events;

namespace ObservabilityExample.Services.Customers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ObservabilityExample.Services.Customers", Version = "v1"});
            });

            services.AddDbContext<CustomerContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging();
            });

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddRabbitMq(Configuration);
            services.AddJaeger(Configuration);
            services.AddPrometheus(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ObservabilityExample.Services.Customers v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseRabbitMq().SubscribeAsync<ProductCreated>(new RabbitMqOptions
                    {ExchangeName = "product", QueueName = "product_to_customer_queue", RoutingKey = "product_created", PrefetchCount = 10});
            app.UsePrometheus();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;
using ObservabilityExample.Services.Products.Controllers;
using ObservabilityExample.Services.Products.Domain;
using ObservabilityExample.Services.Products.Events;

namespace ObservabilityExample.Services.Products.Commands
{
    public class CreateProduct : ICommand, IRequest
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Vendor { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        [JsonConstructor]
        public CreateProduct(Guid id, string name,
                             string description, string vendor,
                             decimal price, int quantity)
        {
            Id = id;
            Name = name;
            Description = description;
            Vendor = vendor;
            Price = price;
            Quantity = quantity;
        }

        public ICorrelationContext CorrelationContext { get; set; }
    }

    public class CreateProductHandler : IRequestHandler<CreateProduct>
    {
        private readonly ProductContext productContext;
        private readonly IBusPublisher busPublisher;
        private readonly ILogger<CreateProductHandler> logger;

        public CreateProductHandler(ProductContext productContext, IBusPublisher busPublisher, ILogger<CreateProductHandler> logger)
        {
            this.productContext = productContext;
            this.busPublisher = busPublisher;
            this.logger = logger;
        }

        public async Task<Unit> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Id, request.Name, request.Description,
                    request.Vendor, request.Price, request.Quantity);

            await productContext.Products.AddAsync(product, cancellationToken);

            await productContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Product is add to {Db}",productContext.Database.GetDbConnection().Database);
            
            await busPublisher.PublishAsync(new ProductCreated(request.Id, request.Name, request.Description,
                            request.Vendor, request.Price, request.Quantity,request.CorrelationContext),
                    new RabbitMqOptions {ExchangeName = "product", RoutingKey = "product_created"},
                    request.CorrelationContext);
            
            return Unit.Value;
        }
    }
}
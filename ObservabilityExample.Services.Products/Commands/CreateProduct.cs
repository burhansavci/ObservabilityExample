using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;
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

        public CreateProductHandler(ProductContext productContext, IBusPublisher busPublisher)
        {
            this.productContext = productContext;
            this.busPublisher = busPublisher;
        }

        public async Task<Unit> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Id, request.Name, request.Description,
                    request.Vendor, request.Price, request.Quantity);

            await productContext.Products.AddAsync(product, cancellationToken);

            await productContext.SaveChangesAsync(cancellationToken);

            await busPublisher.PublishAsync(new ProductCreated(request.Id, request.Name, request.Description,
                            request.Vendor, request.Price, request.Quantity,request.CorrelationContext),
                    new RabbitMqOptions {ExchangeName = "product", RoutingKey = "product_created"},
                    request.CorrelationContext);
            
            return Unit.Value;
        }
    }
}
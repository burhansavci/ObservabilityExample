using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;
using ObservabilityExample.Services.Products.Domain;

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

        public CreateProductHandler(ProductContext productContext)
        {
            this.productContext = productContext;
        }

        public async Task<Unit> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Id, request.Name, request.Description,
                    request.Vendor, request.Price, request.Quantity);

            await productContext.Products.AddAsync(product, cancellationToken);

            await productContext.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}
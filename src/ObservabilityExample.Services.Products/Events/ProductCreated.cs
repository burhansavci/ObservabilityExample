using System;
using MediatR;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;

namespace ObservabilityExample.Services.Products.Events
{
    public class ProductCreated : IRequest, ICommand
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Vendor { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public ICorrelationContext CorrelationContext { get; set; }

        [JsonConstructor]
        public ProductCreated(Guid id, string name,
                              string description, string vendor,
                              decimal price, int quantity, ICorrelationContext correlationContext)
        {
            Id = id;
            Name = name;
            Description = description;
            Vendor = vendor;
            Price = price;
            Quantity = quantity;
            CorrelationContext = correlationContext;
        }
    }
}
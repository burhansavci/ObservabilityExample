﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;
using ObservabilityExample.Services.Customers.Domain;

namespace ObservabilityExample.Services.Customers.Events
{
    public class ProductCreated : IRequest, ICommand
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Vendor { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        [JsonConstructor]
        public ProductCreated(Guid id, string name,
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

    public class ProductCreatedHandler : IRequestHandler<ProductCreated>
    {
        private readonly CustomerContext customerContext;
        private readonly ILogger<ProductCreatedHandler> logger;

        public ProductCreatedHandler(CustomerContext customerContext, ILogger<ProductCreatedHandler> logger)
        {
            this.customerContext = customerContext;
            this.logger = logger;
        }

        public async Task<Unit> Handle(ProductCreated request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Id, request.Name, request.Description,
                    request.Vendor, request.Price, request.Quantity);

            await customerContext.Products.AddAsync(product, cancellationToken);
            
            await customerContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Product is add to {Db}", customerContext.Database.GetDbConnection().Database);
            
            return Unit.Value;
        }
    }
}
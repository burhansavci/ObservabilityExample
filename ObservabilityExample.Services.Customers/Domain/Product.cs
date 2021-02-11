using System;
using ObservabilityExample.Infrastructure.Types;

namespace ObservabilityExample.Services.Customers.Domain
{
    public class Product : BaseEntity
    {
        public string Name { get; }
        public string Description { get; }
        public string Vendor { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        public Product(Guid id, string name, string description,
                       string vendor, decimal price, int quantity) : base(id)
        {
            Name = name;
            Description = description;
            Vendor = vendor;
            Price = price;
            Quantity = quantity;
        }
        
    }
}
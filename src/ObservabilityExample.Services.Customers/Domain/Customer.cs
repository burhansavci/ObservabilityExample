using System;

namespace ObservabilityExample.Services.Customers.Domain
{
    public class Customer
    {
        public Guid Id { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Address { get; }
        public string Country { get; }
        public DateTime CreatedAt { get; }


        public Customer(Guid id, string email, string firstName,
                        string lastName, string address, string country)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Country = country;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
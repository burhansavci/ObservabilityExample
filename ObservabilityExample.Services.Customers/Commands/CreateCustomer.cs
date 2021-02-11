﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;
using ObservabilityExample.Services.Customers.Domain;

namespace ObservabilityExample.Services.Customers.Commands
{
    public class CreateCustomer : ICommand, IRequest
    {
        public Guid Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Address { get; }
        public string Country { get; }
        public string Email { get; }

        [JsonConstructor]
        public CreateCustomer(Guid id, string firstName, string lastName,
                              string address, string country, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Country = country;
            Email = email;
        }

        public ICorrelationContext CorrelationContext { get; set; }
    }

    public class CreateCustomerHandler : IRequestHandler<CreateCustomer>
    {
        private readonly CustomerContext customerContext;

        public CreateCustomerHandler(CustomerContext customerContext)
        {
            this.customerContext = customerContext;
        }

        public async Task<Unit> Handle(CreateCustomer request, CancellationToken cancellationToken)
        {
            var customer = new Customer(request.Id, request.Email, request.FirstName,
                    request.LastName, request.Address, request.Country);

            await customerContext.Customers.AddAsync(customer, cancellationToken);

            await customerContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
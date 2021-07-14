# ObservabilityExample

Simple example to show implementation of observability in .NET 5

### Solution Structure

- #### ObservabilityExample.Infrastructure:

The basics of observability (Tracing, Metrics, Logging) and RabbitMq implementation made. While making these implementations, the following libraries used:

Tracing:  OpenTelemetry's .NET libraries used.

Metrics: In OpenTelemetry, this part is still [under development](https://github.com/open-telemetry/opentelemetry-dotnet/issues/1501), so OpenTelemetry was not used. Instead, the prometheus-net library used.

Logging: Serilog used.

RabbitMq: RawRabbit library used because of its ease of implementation and middleware structure.

- #### ObservabilityExample.Services.Products:

It simply creates a product in its database. Then the ProductCreated event is published to RabbitMq.

- #### ObservabilityExample.Services.Customers:

It simply creates a client in its database. It has also subscribed to the ProductCreated event. When this event comes, it saves a sample of the product in its database.

### TO DOs:

- [ ] Add docker support
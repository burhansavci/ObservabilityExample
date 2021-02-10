using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ObservabilityExample.Infrastructure.RabbitMq;
using ObservabilityExample.Infrastructure.Types;
using ObservabilityExample.Services.Products.Commands;

namespace ObservabilityExample.Services.Products.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProductsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("ping")]
        public IActionResult Get() => Ok("pong");

        [HttpPost]
        public async Task<IActionResult> Post(CreateProduct request)
        {
            request.CorrelationContext = GetContext<CreateProduct>(request.Id, "products");
            await mediator.Send(request);
            return Ok();
        }

        protected ICorrelationContext GetContext<T>(Guid? resourceId = null, string resource = "") where T : ICommand
        {
            if (!string.IsNullOrWhiteSpace(resource))
            {
                resource = $"{resource}/{resourceId}";
            }

            return CorrelationContext.Create<T>(Guid.NewGuid(), resourceId ?? Guid.Empty,
                    HttpContext.TraceIdentifier, HttpContext.Connection.Id, default,
                    Request.Path.ToString(), resource);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;

namespace Ordering.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController: ControllerBase
    {
        private readonly IMediator mediator;

        public OrderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles ="Admin,Otro,Otro2")]
        
        public async Task<ActionResult<List<OrdersViewModel>>>
            GetOrders([FromQuery] GetOrdersListQuery query)
            => await mediator.Send(query);

    }
}

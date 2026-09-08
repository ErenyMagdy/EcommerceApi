using EcommerceApi.Features.Orders.Commands.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController(IMediator _mediator) : ControllerBase
    {
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderResponse>> Checkout(CheckoutCommand request,CancellationToken cancellationToken)
        {
            var orders = await _mediator.Send(request, cancellationToken);
            return Ok(orders);
        }
    }
}

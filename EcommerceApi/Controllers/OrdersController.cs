using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Features.Orders.Commands.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceApi.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value,out var id)
                ? id
                : throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized", "User ID claim missing.");
        }
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderResponse>> Checkout(CheckoutCommand request,CancellationToken cancellationToken)
        {
           request.UserId = GetCurrentUserId();
            var orders = await _mediator.Send(request, cancellationToken);
            return Ok(orders);
        }
    }
}

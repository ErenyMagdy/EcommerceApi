using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Features.Cart.Commands.CreateCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController(IMediator _mediator) : ControllerBase
    {
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}

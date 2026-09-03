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
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id)
                ? id
                : throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized", "User ID claim missing.");
        }
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartCommand command,
            CancellationToken cancellationToken)
        {
            command.UserId = GetCurrentUserId();
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        //private int GetCurrentUserId()
        //{
        //    var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    return claim != null && int.TryParse(claim.Value, out var id) ? id : throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized", "User ID claim missing.");
        //}

        //[HttpGet]
        //public async Task<ActionResult<CartResponseDto>> GetCart()
        //{
        //    var userId = GetCurrentUserId();
        //    var cart = await _cartService.GetUserCartAsync(userId);
        //    return Ok(cart);
        //}

        //[HttpPost("items")]
        //public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        //{
        //    var userId = GetCurrentUserId();
        //    await _cartService.AddItemToCartAsync(userId, dto);
        //    return Ok(new { message = "Item added to cart successfully." });
        //}

        //[HttpDelete("items/{cartItemId:int}")]
        //public async Task<IActionResult> RemoveItem(int cartItemId)
        //{
        //    var userId = GetCurrentUserId();
        //    await _cartService.RemoveItemFromCartAsync(userId, cartItemId);
        //    return NoContent();
        //}
    }
}

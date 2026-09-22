using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Cart.Commands.CreateCart
{
    public class CreateCartCommandHandler(AppDbContext _context, ICurrentUserService _currentUserService) 
        : IRequestHandler<AddToCartCommand, AddToCartResponse>
    {
        public async Task<AddToCartResponse> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized", "يجب تسجيل الدخول لإتمام الطلب.");
            }
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
            if (cart == null)
            {
                cart = new Models.Cart { UserId = (int)userId };
                _context.Carts.Add(cart);
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);

                }
                catch (Exception E)
                {
                    var EX = E.Message;
                    throw E;
                }
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
            if (product == null)
            {
                throw ApiException.NotFound($"Product with ID {request.ProductId} does not exist.");
            }
            if(product.StockQuantity < request.Quantity)
            {
                throw new ApiException(
                    StatusCodes.Status400BadRequest,
                     "Insufficient Stock",
                    $"Only {product.StockQuantity} items available in stock."
                );
            }
            var cartItem = cart.CartItems.FirstOrDefault(ci=>ci.ProductId == request.ProductId);
            if(cartItem != null)
            {
                var newquantity = cartItem.Quantity + request.Quantity;
                if(product.StockQuantity< newquantity)
                {
                    throw new ApiException(
                        StatusCodes.Status400BadRequest, "Insufficient Stock",
                        $"Cannot add more than available stock. Current stock: {product.StockQuantity}"
                    );
                }
                cartItem.Quantity = newquantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                };
                cart.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return new AddToCartResponse
            {
                Message = "Item added to cart successfully.",
                CartItemId = cartItem.Id,
                TotalItems = cart.CartItems.Count,
                UserId = (int)userId,
                CartId = cart.Id
            };
        }
    }
}

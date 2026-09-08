using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Orders.Commands.Checkout
{
    public class CheckoutCommandHandler(AppDbContext _context,
        ICurrentUserService _currentUserService) : IRequestHandler<CheckoutCommand, OrderResponse>
    {
        public async Task<OrderResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized", "يجب تسجيل الدخول لإتمام الطلب.");
            }
            var cart = await _context.Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product).AsNoTracking().FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "Cart is empty",
                    "Cannot checkout an empty shopping cart.");
            }
            var orderItems = new List<OrderItems>();
            var itemResponses = new List<OrderItemResponse>();
            decimal totalAmount = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var product = cartItem.Product;
                var affectedRows = await _context.Products
                    .Where(p => p.Id == product.Id && p.StockQuantity >= cartItem.Quantity)
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        p => p.StockQuantity,
                        p => p.StockQuantity - cartItem.Quantity),
                        cancellationToken);
                if (affectedRows == 0)
                {
                    throw new ApiException(
                        StatusCodes.Status400BadRequest,
                        "Insufficient Stock",
                        $"المنتج '{product.Name}' لم يعد متوفراً بالكمية المطلوبة.");
                }
                orderItems.Add(new OrderItems
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = product.Price
                });
                itemResponses.Add(new OrderItemResponse
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = product.Price
                });
                totalAmount += product.Price * cartItem.Quantity;
            }
            var order = new Order
            {
                UserId = request.UserId,
                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };
            _context.Orders.Add(order);
            await _context.CartItems
               .Where(ci => ci.CartId == cart.Id)
               .ExecuteDeleteAsync(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return new OrderResponse
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = itemResponses
            };
        }
    }
}

using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Orders.Commands.Checkout
{
    public class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, OrderResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CheckoutCommandHandler> _logger;

        public CheckoutCommandHandler(AppDbContext context, ILogger<CheckoutCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<OrderResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts.Include(c=> c.CartItems)
                .ThenInclude(ci=> ci.Product).FirstOrDefaultAsync(c=> c.UserId == request.UserId, cancellationToken);
            if (cart == null)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "Cart is empty",
                    "Cannot checkout an empty shopping cart.");
            }
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var orderItems = new List<OrderItems>();
                var itemResponses = new List<OrderItemResponse>();
                decimal totalAmount = 0;
                foreach (var cartItem in cart.CartItems)
                {
                    var product = cartItem.Product;
                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        throw new ApiException(
                              StatusCodes.Status400BadRequest,
                                "Insufficient Stock",
                                $"Product '{product.Name}' only has {product.StockQuantity} items remaining in stock.");
                    }
                    product.StockQuantity -= cartItem.Quantity;
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

                // Clear cart
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new OrderResponse
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    Items = itemResponses
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

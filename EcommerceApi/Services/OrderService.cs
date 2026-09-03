using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Services
{
    public class OrderService :IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<OrderDto> CheckoutAsync(int userId,
            CancellationToken cancellationToken)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new ApiException(StatusCodes.Status400BadRequest,
                    "Cart is empty", "Cannot checkout an empty shopping cart.");
            }
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var orderItems = new List<OrderItems>();
                var itemResponses = new List<OrderItemDto>();
                decimal totalAmount = 0;
                foreach (var cartItem in cart.CartItems)
                {
                    var product = cartItem.Product;
                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        throw new ApiException(StatusCodes.Status400BadRequest, "Insufficient Stock",
                            $"Product '{product.Name}' only has {product.StockQuantity} items remaining in stock.");
                    }
                    product.StockQuantity -= cartItem.Quantity;
                    orderItems.Add(new OrderItems
                    {
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        PriceAtPurchase = product.Price
                    });
                    itemResponses.Add(new OrderItemDto
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
                    UserId = userId,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    OrderItems = orderItems
                };
                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new OrderDto
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
        public async Task<List<OrderDto>> GetUserOrdersAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.Orders.Where(o => o.UserId == userId)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt).Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        PriceAtPurchase = oi.PriceAtPurchase,
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}

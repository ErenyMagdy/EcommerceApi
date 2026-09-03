using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Services
{
    public class CartService: ICartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CartResponseDto> GetUserCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                throw ApiException.NotFound("Cart not found for this user.");
            }

            return new CartResponseDto
            {
                Id = cart.Id,
                Items = cart.CartItems.Select(ci => new CartItemResponseDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity
                }).ToList()
            };
        }
        public async Task AddItemToCartAsync(int userId, AddToCartDto dto)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync (c => c.UserId == userId);
            if(cart == null)
            {
                throw ApiException.NotFound("Cart not found.");
            }
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
            {
                throw ApiException.NotFound($"Product with ID {dto.ProductId} does not exist");
            }
            if(product.StockQuantity< dto.Quantity)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "Insufficient Stock", $"Only {product.StockQuantity} items available in stock.");
            }
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (cartItem == null)
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }
            else
            {
                var newQuantity = cartItem.Quantity + dto.Quantity;
                if (product.StockQuantity < newQuantity)
                { 
                    throw new ApiException(StatusCodes.Status400BadRequest, "Insufficient Stock", "Cannot add more than available stock.");
                }
                cartItem.Quantity = newQuantity;
            }
            await _context.SaveChangesAsync();
        }
        public async Task RemoveItemFromCartAsync(int userId, int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                throw ApiException.NotFound("Cart not found.");
            }

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
            {
                throw ApiException.NotFound("Cart item not found.");
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}

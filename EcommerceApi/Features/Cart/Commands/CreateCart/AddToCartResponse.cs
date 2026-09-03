using EcommerceApi.Models;

namespace EcommerceApi.Features.Cart.Commands.CreateCart
{
    public class AddToCartResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CartItemId { get; set; }
        public int TotalItems { get; set; }
        //public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

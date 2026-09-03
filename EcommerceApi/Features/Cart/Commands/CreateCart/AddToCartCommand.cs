using EcommerceApi.Features.Base;
using EcommerceApi.Models;

namespace EcommerceApi.Features.Cart.Commands.CreateCart
{
    public class AddToCartCommand:ICommand<AddToCartResponse>
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

namespace EcommerceApi.Dtos
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    public class CartResponseDto
    {
        public int Id { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
        public decimal GrandTotal => Items.Sum(i => i.TotalPrice);
    }
}

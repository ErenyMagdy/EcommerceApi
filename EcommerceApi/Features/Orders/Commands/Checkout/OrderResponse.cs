namespace EcommerceApi.Features.Orders.Commands.Checkout
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}

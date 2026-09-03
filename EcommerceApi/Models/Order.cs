using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Paid, Shipped, Cancelled
        public string? PaymentIntentId { get; set; } // Stripe/PayPal transaction reference
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}

namespace EcommerceApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer"; // Customer or Admin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEndUtc { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

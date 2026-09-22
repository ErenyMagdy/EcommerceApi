namespace EcommerceApi.Features.Products.Commands.CreateProduct
{
    public class CreateProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
    }
}

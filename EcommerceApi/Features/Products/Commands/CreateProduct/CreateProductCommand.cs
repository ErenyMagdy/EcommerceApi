using EcommerceApi.Features.Base;

namespace EcommerceApi.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : ICommand<CreateProductResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
    }
}

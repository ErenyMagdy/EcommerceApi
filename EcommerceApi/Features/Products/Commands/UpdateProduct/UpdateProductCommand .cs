using EcommerceApi.Features.Base;

namespace EcommerceApi.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : ICommand<UpdateProductResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}

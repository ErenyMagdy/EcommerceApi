
using EcommerceApi.Features.Base;

namespace EcommerceApi.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand :ICommand<bool>
    {
        public int Id { get; set; }
    }
}

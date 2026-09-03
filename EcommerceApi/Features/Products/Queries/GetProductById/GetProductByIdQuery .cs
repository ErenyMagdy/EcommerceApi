using EcommerceApi.Dtos;
using EcommerceApi.Features.Base;

namespace EcommerceApi.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IQuery<ProductDto>
    {
        public int Id { get; set; }

    }
}

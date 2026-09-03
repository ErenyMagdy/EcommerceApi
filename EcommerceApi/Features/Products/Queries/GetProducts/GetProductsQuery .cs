using EcommerceApi.Dtos;
using EcommerceApi.Features.Base;

namespace EcommerceApi.Features.Products.Queries.GetProducts
{
    public class GetProductsQuery : IQuery<PagedResult<ProductDto>>
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

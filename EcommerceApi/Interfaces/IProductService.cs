using EcommerceApi.Dtos;
using EcommerceApi.Models;

namespace EcommerceApi.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<Product>> GetProductsAsync(QueryParameters queryParameters, CancellationToken cancellationToken);
        Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<Product> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken);
        Task UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken);
    }
}

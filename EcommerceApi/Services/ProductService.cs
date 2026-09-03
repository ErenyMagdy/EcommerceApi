using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Services
{
    public class ProductService:IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Product>> GetProductsAsync( QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                var searchTerm = queryParameters.SearchTerm.Trim();
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                p.Description.Contains(searchTerm));
            }
            if (!string.IsNullOrWhiteSpace(queryParameters.SortBy))
            {
                query = queryParameters.SortBy.ToLower() switch
                {
                    "price" => queryParameters.IsDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "name" => queryParameters.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync(cancellationToken);
            return new PagedResult<Product>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                TotalCount = totalCount,
            };
        }
        public async Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(
                product => product.Id == id,cancellationToken);
            if (product == null)
            {
                throw ApiException.NotFound($"Product with ID {id} was not found. ");
            }
            return product;
        }
        public async Task<Product> CreateProductAsync( CreateProductDto createProduct, CancellationToken cancellationToken = default)
        {
            var product = new Product
            {
                Name = createProduct.Name,
                Description = createProduct.Description,
                Price = createProduct.Price,
                StockQuantity = createProduct.StockQuantity,
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }
        public async Task UpdateProductAsync(int id,UpdateProductDto updateProduct, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw ApiException.NotFound($"Product with ID {id} was not found.");
            }
            product.Name = updateProduct.Name;
            product.Description = updateProduct.Description;
            product.Price = updateProduct.Price;
            product.StockQuantity = updateProduct.StockQuantity;
            await _context.SaveChangesAsync(cancellationToken);
           
        }
        public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw ApiException.NotFound($"Product with ID {id} was not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

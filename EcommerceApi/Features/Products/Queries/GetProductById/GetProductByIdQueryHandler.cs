using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler(AppDbContext _context) : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.AsNoTracking()
                .Where(p=>p.Id == request.Id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Sku = p.Sku,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (product == null)
            {
                throw ApiException.NotFound($"Product with ID {request.Id} was not found");
            }
            return product;
        }
    }
}

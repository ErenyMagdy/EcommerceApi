using Azure.Core;
using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler:IQueryHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(AppDbContext context, ILogger<GetProductByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
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

using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(AppDbContext context, ILogger<CreateProductCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var sku = request.Sku?? GenerateSku(request.Name);
            var existingProduct = await _context.Products
                .AnyAsync(p=> p.Sku == sku, cancellationToken);
            if (existingProduct)
            {
                throw new ApiException(
                   StatusCodes.Status409Conflict,
                   "Duplicate SKU",
                   $"A product with SKU '{sku}' already exists."
               );
            }
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                Sku = sku,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) //  Catch concurrent race conditions on the unique SKU index
            {
                throw new ApiException(
                   StatusCodes.Status409Conflict,
                   "Duplicate SKU",
                   $"A product with SKU '{sku}' already exists."
               );
            }
            return new CreateProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }
        private string GenerateSku(string name)
        {
            var prefix = string.Concat(name.Take(3));
            var timestamp = DateTime.UtcNow.Ticks.ToString().Substring(0, 8);
            return $"{prefix}-{timestamp}";
        }
    }
}

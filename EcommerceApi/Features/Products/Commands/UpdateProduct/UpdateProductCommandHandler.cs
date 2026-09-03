using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using Mapster;
namespace EcommerceApi.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, UpdateProductResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        public UpdateProductCommandHandler(AppDbContext context, ILogger<UpdateProductCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id, cancellationToken);
            if (product == null)
                throw ApiException.NotFound($"Product with ID {request.Id} was not found.");
            request.Adapt(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product.Adapt<UpdateProductResponse>();
        }

    }
}

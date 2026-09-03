using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductHandler: ICommandHandler<DeleteProductCommand,bool>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DeleteProductHandler> _logger;
        public DeleteProductHandler(AppDbContext context, ILogger<DeleteProductHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id, cancellationToken);
            if (product == null)
                throw ApiException.NotFound($"Product with Id {request.Id} not found.");

            var hasOrder = await _context.OrderItems.AnyAsync(oi=>oi.ProductId == request.Id, cancellationToken);
            if (hasOrder)
            {
                throw new ApiException(StatusCodes.Status409Conflict, "Product in Use",
                    "This product cannot be deleted because it's referenced in existing orders.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

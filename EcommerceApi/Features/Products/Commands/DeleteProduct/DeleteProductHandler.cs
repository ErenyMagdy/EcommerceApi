using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductHandler(AppDbContext _context) : IRequestHandler<DeleteProductCommand,bool>
    {
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

using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using MediatR;

namespace EcommerceApi.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>(AppDbContext _context) 
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is ICommand<TResponse>)
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var response = await next();
                    await transaction.CommitAsync(cancellationToken);
                    return response;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            return await next();
        }
    }
}

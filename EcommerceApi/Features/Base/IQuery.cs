using MediatR;

namespace EcommerceApi.Features.Base
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult> { }
}

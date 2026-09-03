using MediatR;

namespace EcommerceApi.Features.Base
{
    public interface ICommand<out TResult> :IRequest<TResult>
    {
    }
    public interface ICommandHandler<TCommand, TResult>: IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
    }
}

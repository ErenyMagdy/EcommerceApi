using FluentValidation;
using MediatR;

namespace EcommerceApi.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> _validator) 
        : IPipelineBehavior<TRequest, TResponse>
    where TRequest:IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validator.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validator.Select(v => v.ValidateAsync(context, cancellationToken))
                    );
                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(r => r != null).ToList();
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
            }
            var response = await next();
            return response;
        }
    }
}

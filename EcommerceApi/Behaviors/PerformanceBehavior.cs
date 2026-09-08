using MediatR;
using System.Diagnostics;

namespace EcommerceApi.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> _logger,
        Stopwatch _timer) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request,RequestHandlerDelegate<TResponse> next,
                    CancellationToken cancellationToken)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;

            if (elapsedMilliseconds > 3000)
                _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                    requestName, elapsedMilliseconds, request);

            return response;
        }
    }
}

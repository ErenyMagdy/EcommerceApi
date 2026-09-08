using MediatR;
using System.Diagnostics;

namespace EcommerceApi.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> _logger) 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
      
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Starting {RequestName} at {Time}",
               requestName, DateTime.UtcNow);
            try
            {
                var response = await next();
                stopwatch.Stop();
                _logger.LogInformation("Completed {RequestName} in {Elapsed}ms",
                   requestName, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed {RequestName} after {Elapsed}ms",
                   requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

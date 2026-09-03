using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;
            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "Internal server error";
            var detail = "An unexpected error occured. Please try again later.";
            if (exception is ApiException apiException)
            {
                statusCode = apiException.StatusCode;
                title = apiException.Title;
                detail = apiException.Message;
                _logger.LogWarning(
                    "Handled API exception for {Method} {Path}." +
                    "StatusCode: {StatusCode}, TraceId: {TraceId}, Detail: {Detail}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    statusCode,
                    traceId,
                    detail);
            }
            else
            {
                _logger.LogError(exception, "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                    httpContext.Request.Method, httpContext.Request.Path, traceId);
            }
            httpContext.Response.StatusCode = statusCode;
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = traceId;
            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            };
            var responseWritten = await _problemDetailsService.TryWriteAsync(
                problemDetailsContext);
            if (!responseWritten)
            {
                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            }
            return true;

        }

    }
}

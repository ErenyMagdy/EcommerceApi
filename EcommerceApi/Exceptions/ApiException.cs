using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string Title { get; }

        public ApiException(
            int statusCode,
            string title,
            string detail)
            : base(detail)
        {
            StatusCode = statusCode;
            Title = title;
        }
        public static ApiException NotFound(string detail)
        {
            return new ApiException(
                StatusCodes.Status404NotFound, "Resource not found", detail);
        }
        public static ApiException Conflict (string detail)
        {
            return new ApiException(
                StatusCodes.Status409Conflict,
                "Conflict", detail);
        }
    }
}

using ShopOnline.Api.Models;
using System.Net;
using System.Text.Json;

namespace ShopOnline.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        /* ILogger 
        ** Günlüğe kaydetme gerçekleştirmek için kullanılan bir türü temsil eder.
        ** logger kullanımı => _logger 
        */
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, 
                                           ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occured");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                // ÖNCE spesifik exception'ları kontrol et
                // Eğer exception ArgumentNullException ise
                ArgumentNullException nullEx => new ErrorResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Required Data Missing",
                    nullEx.Message,
                    context.Request.Path
                ),

                // SONRA genel exception'ları kontrol et
                // Eğer exception ArgumentException ise
                ArgumentException argEx => new ErrorResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Validation Error",
                    argEx.Message,
                    context.Request.Path
                ),

                // Eğer exception KeyNotFoundException ise
                KeyNotFoundException notFoundEx => new ErrorResponse(
                    (int)HttpStatusCode.NotFound,
                    "Resource Not Found",
                    notFoundEx.Message,
                    context.Request.Path
                ),

                UnauthorizedAccessException unauthorizedEx => new ErrorResponse(
                    (int)HttpStatusCode.Unauthorized,
                    "Unauthorized Access",
                    unauthorizedEx.Message,
                    context.Request.Path
                ),

                InvalidOperationException invalidOpEx => new ErrorResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Invalid Operation",
                    invalidOpEx.Message,
                    context.Request.Path
                ),

                // Diğer tüm exception'lar için (default)
                _ => new ErrorResponse(
                    (int)HttpStatusCode.InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    context.Request.Path
                )
            };

            context.Response.StatusCode = errorResponse.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync( jsonResponse );
        }
    }
}

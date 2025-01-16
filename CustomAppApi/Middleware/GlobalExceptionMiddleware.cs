using System.Net;
using System.Text.Json;
using CustomAppApi.Models.Common;
using FluentValidation;

namespace CustomAppApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                
                response.StatusCode = error switch
                {
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    InvalidOperationException => (int)HttpStatusCode.BadRequest,
                    FluentValidation.ValidationException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError,
                };

                if (error is FluentValidation.ValidationException validationEx)
                {
                    var errorResult = JsonSerializer.Serialize(ApiResponse<object>.ErrorResult(
                        "Validation failed", 
                        validationEx.Errors.Select(e => e.ErrorMessage).ToList()));
                    await response.WriteAsync(errorResult);
                    return;
                }

                var finalResult = JsonSerializer.Serialize(ApiResponse<object>.ErrorResult(error.Message));
                await response.WriteAsync(finalResult);
            }
        }
    }
} 
using System.Net;
using System.Text.Json;
using CustomAppApi.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace CustomAppApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
                
                var errorResponse = new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Errors = new List<string>()
                };

                switch (error)
                {
                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        errorResponse.Message = error.Message;
                        break;

                    case InvalidOperationException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorResponse.Message = error.Message;
                        break;

                    case UnauthorizedAccessException:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        errorResponse.Message = error.Message;
                        break;

                    case FluentValidation.ValidationException validationEx:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorResponse.Message = "Validation failed";
                        errorResponse.Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                        break;

                    case DbUpdateException dbEx:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorResponse.Message = "Database error occurred";
                        errorResponse.Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message };
                        _logger.LogError(dbEx, "Database error occurred");
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse.Message = "An unexpected error occurred";
                        errorResponse.Errors = new List<string> { error.Message };
                        _logger.LogError(error, "An unexpected error occurred");
                        break;
                }

                await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
} 
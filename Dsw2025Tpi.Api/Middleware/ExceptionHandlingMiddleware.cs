using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Dsw2025Tpi.Application.Exceptions;

namespace Dsw2025Tpi.Application.Middleware
{
    public class ExceptionHandlingMiddleware
    {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionHandlingMiddleware> _logger;

            public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                catch (Exception ex)
                {
                    _logger.LogError($"Se ha producido un error: {ex.Message}");
                    await HandleExceptionAsync(context, ex);
                }
            }

            private static Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                int statusCode = (int)HttpStatusCode.InternalServerError;
                string message = "Se produjo un error interno.";

                if (exception is EntityNotFoundException || exception is KeyNotFoundException)
                {
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = exception.Message;
                }
                else if (exception is BusinessRuleViolationException || exception is ArgumentException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = exception.Message;
                }

                var result = JsonSerializer.Serialize(new
                {
                    statusCode,
                    message
                });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                return context.Response.WriteAsync(result);
            }
        }
    }




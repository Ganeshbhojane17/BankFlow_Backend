using CustomerService.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace CustomerService.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception. TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleExceptionAsync(
                context,
                ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode =
            exception switch
            {
                NotFoundException =>
                    HttpStatusCode.NotFound,

                ConflictException =>
                    HttpStatusCode.Conflict,

                ValidationException =>
                    HttpStatusCode.BadRequest,

                _ =>
                    HttpStatusCode.InternalServerError
            };

        context.Response.StatusCode =
            (int)statusCode;

        context.Response.ContentType =
            "application/json";

        var response = new
        {
            success = false,

            message = exception switch
            {
                NotFoundException =>
                    exception.Message,

                ConflictException =>
                    exception.Message,

                ValidationException =>
                    exception.Message,

                _ =>
                    "An unexpected error occurred."
            },

            correlationId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
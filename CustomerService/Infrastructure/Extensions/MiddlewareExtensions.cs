using CustomerService.Infrastructure.Middleware;

namespace CustomerService.Infrastructure.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        //app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }
}
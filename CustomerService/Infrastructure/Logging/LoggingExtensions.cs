namespace CustomerService.Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static IApplicationBuilder
            UseRequestLogging(
                this IApplicationBuilder app)
        {
            return app.UseMiddleware<
                RequestLoggingMiddleware>();
        }
    }
}

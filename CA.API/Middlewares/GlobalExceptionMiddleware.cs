using CA.Infrastructures.Logging;
using CA.Infrastructures.Persistence;
using Microsoft.AspNetCore.Http;
using Volo.Abp;

namespace CA.API.Middlewares
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await LogToDatabaseAsync(ex, context,StatusCodes.Status500InternalServerError);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal server error",
                    traceId = context.TraceIdentifier
                });
            }
        }

        private async Task LogToDatabaseAsync(Exception ex, HttpContext context,int statusCode)
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var log = new SystemExceptionLog(
                applicationName: "LoanService", // nên lấy từ config
                serviceName: context.Request.Host.Value,
                message: ex.Message,
                requestPath: context.Request.Path,
                httpMethod: context.Request.Method,
                statusCode: statusCode
            );

            dbContext.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}

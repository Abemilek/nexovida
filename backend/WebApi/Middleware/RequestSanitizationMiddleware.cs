using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApi.Middleware
{
    public class RequestSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestSanitizationMiddleware> _logger;

        public RequestSanitizationMiddleware(RequestDelegate next, ILogger<RequestSanitizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString("N");
            
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey("X-Request-Id"))
                {
                    context.Response.Headers.Append("X-Request-Id", requestId);
                }
                return Task.CompletedTask;
            });

            var user = context.User?.Identity?.IsAuthenticated == true 
                ? context.User.Identity.Name ?? "Authenticated User" 
                : "Anonymous";

            _logger.LogInformation("Request {RequestId}: {Method} {Path} by {User}", 
                requestId, context.Request.Method, context.Request.Path, user);

            await _next(context);
        }
    }

    public static class RequestSanitizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestSanitization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestSanitizationMiddleware>();
        }
    }
}

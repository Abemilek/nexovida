using System.Net;
using System.Text.Json;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString("N");

                _logger.LogError(
                    ex,
                    "Excepcion no controlada. CorrelationId: {CorrelationId}, Ruta: {Path}",
                    correlationId,
                    context.Request.Path
                );

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var body = _environment.IsDevelopment()
                    ? new
                    {
                        message = "Ocurrio un error interno procesando la solicitud.",
                        correlationId,
                        detail = ex.Message,
                    }
                    : (object)new
                    {
                        message = "Ocurrio un error interno procesando la solicitud.",
                        correlationId,
                    };

                await context.Response.WriteAsync(JsonSerializer.Serialize(body));
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}

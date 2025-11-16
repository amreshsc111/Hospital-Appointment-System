using System.Text.Json;

namespace HAS.API.Middleware;

public static class ExceptionMidddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch (FluentValidation.ValidationException vex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = vex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Validation failed", details = errors }));
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
            }
        });

        return app;
    }
}

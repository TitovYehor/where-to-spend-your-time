using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace WhereToSpendYourTime.Api.Handlers;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        this._logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred at {Time}", DateTime.UtcNow);

        httpContext.Response.StatusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            error = exception.Message,
            type = exception.GetType().Name
        };

        var json = JsonSerializer.Serialize(response);
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}

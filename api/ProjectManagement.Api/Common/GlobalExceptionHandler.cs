using Microsoft.AspNetCore.Diagnostics;

namespace ProjectManagement.Api.Common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception");
        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/json";

        var result = Result.Fail("INTERNAL_ERROR", "An unexpected error occurred");
        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}

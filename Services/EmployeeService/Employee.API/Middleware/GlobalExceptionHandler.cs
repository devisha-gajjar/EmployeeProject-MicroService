using System.Diagnostics;
using Employee.Shared.Extensions;
using Microsoft.AspNetCore.Diagnostics;

namespace Employee.API.Middleware;
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        var queryString = httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : string.Empty;

        _logger.LogError(exception,
            "An error occurred while processing request {Method} {Path} on {MachineName} with trace ID : {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path + queryString,
            Environment.MachineName,
            traceId);

        var (statusCode, message) = ExceptionMapper.Map(exception);

        var response = new
        {
            Result = false,
            Message = message,
            StatusCode = statusCode,
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
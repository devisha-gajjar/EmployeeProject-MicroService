namespace ApiGateway.Middleware;

public class TenantForwardingMiddleware(RequestDelegate next, ILogger<TenantForwardingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // JWT already validated by UseAuthentication() above
        var tenantId = context.User?.FindFirst("tenant_id")?.Value;

        if (!string.IsNullOrEmpty(tenantId))
        {
            context.Request.Headers["X-Tenant-ID"] = tenantId;
            context.Request.Headers["X-Forwarded-By"] = "ApiGateway";

            logger.LogInformation("[Gateway] {Method} {Path} → TenantId: {TenantId}",
                context.Request.Method, context.Request.Path, tenantId);
        }

        await next(context);
    }
}
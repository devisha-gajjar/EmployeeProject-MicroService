namespace ApiGateway.Middleware;

public class TenantForwardingMiddleware(RequestDelegate next, ILogger<TenantForwardingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = context.User?.FindFirst("Tenant Id")?.Value;

        var schemaName = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(tenantId) || !string.IsNullOrEmpty(schemaName))
        {
            if (!string.IsNullOrEmpty(tenantId))
                context.Request.Headers["X-Tenant-ID"] = tenantId;

            if (!string.IsNullOrEmpty(schemaName))
                context.Request.Headers["X-Tenant-Schema"] = schemaName;

            context.Request.Headers["X-Forwarded-By"] = "ApiGateway";

            logger.LogInformation("[Gateway] {Method} {Path} → ID: {TenantId}, Schema: {SchemaName}",
                context.Request.Method, context.Request.Path, tenantId, schemaName);
        }

        await next(context);
    }
}
using Employee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Employee.API.Middleware;

public class TenantSchemaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantDbContext dbContext)
    {
        if (context.Request.Headers.TryGetValue("X-Tenant-Schema", out var schemaName))
        {
            // 1. Manually open the connection if it isn't open
            // This ensures the SET search_path "sticks" to this specific DB context instance
            if (dbContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
            {
                await dbContext.Database.OpenConnectionAsync();
            }

            var sql = $"SET search_path TO \"{schemaName}\", public";
            await dbContext.Database.ExecuteSqlRawAsync(sql);
        }

        await next(context);
    }
}
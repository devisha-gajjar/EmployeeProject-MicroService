using System.Text.RegularExpressions;
using Employee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Employee.API.Middleware;

public partial class TenantSchemaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantDbContext dbContext)
    {
        if (context.Request.Headers.TryGetValue("X-Tenant-Schema", out var schemaNameValues))
        {
            var schemaName = schemaNameValues.ToString();

            if (!MyRegex().IsMatch(schemaName))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid tenant schema format.");
                return;
            }

            if (dbContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
            {
                await dbContext.Database.OpenConnectionAsync();
            }

            var searchPathValue = $"{schemaName}, public";

            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT set_config('search_path', {0}, false);",
                searchPathValue
            );
        }

        await next(context);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_]+$")]
    private static partial Regex MyRegex();

}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tenant.Api.Features.Tenants.Commands;
using Tenant.Infrastructure.Data.Host;
using Tenant.Infrastructure.Data.Tenant;
using Tenant.API.Features.Tenants.Handler;
using Tenant.API.Extensions;
using Tenant.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection String
var connectionString = builder.Configuration.GetConnectionString("DbConnection");

// 2. Register HostDbContext (The Master Registry)
builder.Services.AddDbContext<HostDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Register TenantDbContext (The Business Template)
// We add the ModelCacheKeyFactory here to allow schema switching at runtime
builder.Services.AddDbContext<TenantDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
});

// 4. Register DbContextOptions specifically
// This is required so the Handler can manually create TenantDbContext instances
// builder.Services.AddSingleton(new DbContextOptionsBuilder<TenantDbContext>()
//     .UseNpgsql(connectionString)
//     .Options);

builder.Services.AddApplicationServices(builder.Configuration);

// 5. MediatR Registration
// IMPORTANT: Point this to the assembly where your HANDLER lives
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTenantHandler).Assembly));

// 6. Swagger & Tools
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTenantEndpoints();

app.MapGet("/", () => "Tenant Service API Running 🚀");
app.Run();

// --- Support Classes ---

// Put this at the bottom of Program.cs or in your Infrastructure project
public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is TenantDbContext tenantContext)
        {
            // This ensures EF Core creates a fresh model for every unique schema
            return (context.GetType(), tenantContext.SchemaName, designTime);
        }
        return context.GetType();
    }
}
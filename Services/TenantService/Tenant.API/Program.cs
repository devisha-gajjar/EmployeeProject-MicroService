using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tenant.Infrastructure.Data.Host;
using Tenant.Infrastructure.Data.Tenant;
using Tenant.API.Features.Tenants.Handler;
using Tenant.API.Extensions;
using Tenant.API.Endpoints;
using Tenant.Application.Common;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

// Register HostDbContext (Master)
builder.Services.AddDbContext<HostDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register TenantDbContext
builder.Services.AddDbContext<TenantDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
});

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(ApplicationAssemblyMarker));

// MediatR Registration
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTenantHandler).Assembly));

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

public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is TenantDbContext tenantContext)
        {
            // ensures EF Core creates a fresh model for every unique schema
            return (context.GetType(), tenantContext.SchemaName, designTime);
        }
        return context.GetType();
    }
}
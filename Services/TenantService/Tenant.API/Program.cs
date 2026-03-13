using MediatR;
using Tenant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Tenant.Api.Features.Tenants.Commands; // Ensure this matches your namespace

var builder = WebApplication.CreateBuilder(args);

// 1. Register Swagger/OpenAPI Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTenantCommand).Assembly));

var app = builder.Build();

// 2. Enable Swagger Middleware (Place this before Endpoints)
// Removing the 'if' check so it definitely works for your test now
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty; // This makes Swagger open at http://localhost:5236/ directly
});

// Minimal API Endpoint
app.MapPost("/api/tenants", async (CreateTenantCommand command, IMediator mediator) =>
{
    try
    {
        var schemaName = await mediator.Send(command);
        return Results.Created($"/api/tenants/{schemaName}", new { Schema = schemaName });
    }
    catch (Exception ex)
    {
        // Helpful for debugging if the schema creation fails
        return Results.BadRequest(new { Error = ex.Message, Inner = ex.InnerException?.Message });
    }
});

app.Run();
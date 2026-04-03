using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tenant.Infrastructure.Data.Host;
using Tenant.Infrastructure.Data.Tenant;
using Tenant.API.Features.Tenants.Handler;
using Tenant.API.Extensions;
using Tenant.API.Endpoints;
using Tenant.Application.Common;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Employee.Shared.Exceptions;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth API",
        Version = "v1"
    });

    // JWT Support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {your JWT token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var jwtSecret =
    builder.Configuration["JWT_SECRET"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException("JWT Secret not configured");

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtSecret)
);


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Token from Cookie
                if (context.Token == null &&
                    context.Request.Cookies.ContainsKey("Token"))
                {
                    context.Token = context.Request.Cookies["Token"];
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = signingKey,
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTenantEndpoints();

app.MapGet("/", () => "Tenant Service API Running 🚀");
await app.RunAsync();

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
using Dashboard.API.Services;
using Dashboard.Application.Interfaces;
using Dashboard.Domain.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dashboard API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {token}",
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSecret =
    builder.Configuration["JWT_SECRET"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(DashboardMappingProfile).Assembly);

// gRPC Client
builder.Services.AddGrpcClient<Employee.API.Protos.EmployeeGrpc.EmployeeGrpcClient>(o =>
    o.Address = new Uri("http://localhost:5001"));

builder.Services.AddScoped<IEmployeeService, EmployeeGrpcClient>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dashboard V1");
});

app.UseAuthentication();
app.UseAuthorization();

// Endpoints 
app.MapGet("/api/user/dashboard", async (IEmployeeService employeeService) =>
{
    try
    {
        var result = await employeeService.GetEmployeeDashboardDataAsync();
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Dashboard Error: {ex.Message}");
        return Results.Problem("Error fetching data from Employee Service.");
    }
})
.WithName("GetEmployeeDashboard")
.RequireAuthorization();

app.Run();
using Employee.API.Endpoints;
using Employee.API.Extensions;
using Employee.API.Middleware;
using Employee.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Employee.Application.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

// Global/App Context (Public Schema)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppConnection")));

// Database Context
builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TenantConnection")));

// Register MediatR here. 
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// custom extension for other services 
builder.Services.ServiceClass(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    var isDev = builder.Environment.IsDevelopment();
    options.InvalidModelStateResponseFactory = context =>
        ValidationResponseHelper.CreateValidationErrorResponse(context, isDev);
});

builder.Services.AddGrpcReflection();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGrpcReflectionService();
}

// Middleware
app.UseExceptionHandler();

// Custom Middleware 
app.UseMiddleware<TenantSchemaMiddleware>();

app.MapGet("/", () => "Auth API Running 🚀");

app.MapGrpcService<EmployeeGrpcService>();
app.MapEmployeeEndpoints();
app.MapDepartmentEndpoints();

await app.RunAsync();
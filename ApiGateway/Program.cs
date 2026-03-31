using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["JWT_SECRET"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new Exception("JWT Secret not configured");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure YARP with Request Transforms
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            var user = transformContext.HttpContext.User;

            // Debug Log
            Console.WriteLine($"[Debug] Path: {transformContext.HttpContext.Request.Path} | Auth: {user.Identity?.IsAuthenticated} | Claims: {user.Claims.Count()}");

            if (user.Identity?.IsAuthenticated == true)
            {
                var tenantId = user.FindFirst("Tenant Id")?.Value;
                var schemaName = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(tenantId))
                {
                    Console.WriteLine($"[Gateway] Found Tenant: {tenantId}");
                    transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Tenant-ID", tenantId);
                }

                if (!string.IsNullOrEmpty(schemaName))
                {
                    transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Tenant-Schema", schemaName);
                }

                transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-By", "ApiGateway");
            }
            return ValueTask.CompletedTask;
        });
    });

// Configure Authentication to use the Signing Key (Not Authority)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("Token"))
                {
                    context.Token = context.Request.Cookies["Token"];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // This forces YARP to return 401 if the token is invalid/expired
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRateLimiter(options => { });

var app = builder.Build();

app.UseRateLimiter();
app.UseCors("ReactAppPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Request.Headers.Remove("X-Tenant-ID");
    context.Request.Headers.Remove("X-Tenant-Schema");
    await next();
});

app.MapReverseProxy();
app.Run();
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// 1. Get Secret Key (Must match your TokenService secret exactly)
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

// 2. Configure YARP with Request Transforms
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            var user = transformContext.HttpContext.User;

            // Helpful Debug Log
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

// 3. Configure Authentication to use the Signing Key (Not Authority)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],     // Must be "JwtIssuerDevisha"
            ValidAudience = builder.Configuration["Jwt:Audience"], // Must be "JwtAudienceDevisha"
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Pull token from Cookie if header is missing
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
builder.Services.AddRateLimiter(options => { /* your existing config */ });

var app = builder.Build();

// 4. Pipeline Order is critical!
app.UseRateLimiter();
app.UseCors("ReactAppPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Optional: Security cleanup
app.Use(async (context, next) =>
{
    context.Request.Headers.Remove("X-Tenant-ID");
    context.Request.Headers.Remove("X-Tenant-Schema");
    await next();
});

app.MapReverseProxy();
app.Run();
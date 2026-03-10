namespace Auth.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", AuthHandlers.Register);
        group.MapPost("/login", AuthHandlers.Login)
             .RequireRateLimiting("loginLimit");
        group.MapPost("/refresh", AuthHandlers.Refresh);
        group.MapPost("/verify-2fa", AuthHandlers.Verify2FA);
        group.MapPost("/2fa/setup", AuthHandlers.Setup2FA)
             .RequireAuthorization();
    }
}
using Auth.Application.ServiceInterfaces;
using Auth.Application.Services;
using Auth.Infrastructure.Repositories;
using Employee.Shared.Interfaces;

namespace Auth.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        // Application Services
        services.AddScoped<IAuthService, AuthService>();

        // Infrastructure Services
        services.AddScoped<ICustomService, CustomService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();

        // Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
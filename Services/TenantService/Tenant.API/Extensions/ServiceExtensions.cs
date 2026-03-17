using Employee.Shared.Interfaces;
using Employee.Shared.Repositories;
using Tenant.Application.Interface;
using Tenant.Application.Services;
using Tenant.Infrastructure.Interfaces;
using Tenant.Infrastructure.Repository;

namespace Tenant.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
      this IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddScoped<ITenantUnitOfWork, TenantUnitOfWork>();

        // Application Services
        services.AddScoped<ITenantRegistrationService, TenantRegistrationService>();

        return services;
    }
}

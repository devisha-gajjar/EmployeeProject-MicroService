using Employee.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Tenant.Api.Features.Tenants.Commands;
using Tenant.Application.Interface;
using Tenant.Domain.Dtos;
using Tenant.Infrastructure.Data.Host;
using Tenant.Infrastructure.Data.Tenant;

namespace Tenant.API.Features.Tenants.Handler;

public class CreateTenantHandler(
    HostDbContext hostContext,
    DbContextOptions<TenantDbContext> tenantOptions, ITenantRegistrationService registrationService)
    : IRequestHandler<CreateTenantCommand, string>
{
    public async Task<string> Handle(CreateTenantCommand request, CancellationToken cancellationToken)  
    {
        var schemaName = $"tenant_{request.CompanyName.ToLower().Replace(" ", "_")}";

        // Create physical schema
        await hostContext.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", cancellationToken);

        // Setup Tenant Context for the NEW schema
        using var tenantContext = new TenantDbContext(tenantOptions, schemaName);

        // Create the tables
        var databaseCreator = tenantContext.GetService<IRelationalDatabaseCreator>();
        await databaseCreator.CreateTablesAsync(cancellationToken);

        var tenantDto = new CreateTenantDto
        {
            CompanyName = request.CompanyName,
            AdminEmail = request.AdminEmail,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
            SchemaName = schemaName
        };

        var success = registrationService.CreateTenantAsync(tenantDto);

        if (!success)
        {
            throw new AppException("Failed to register the tenant in the master database");
        }

        return schemaName;
    }
}
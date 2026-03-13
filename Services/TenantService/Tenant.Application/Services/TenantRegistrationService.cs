
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Tenant.Infrastructure.Data;

namespace Tenant.Application.Services;

public class TenantRegistrationService(TenantDbContext context)
{
    public async Task CreateTenantWithTablesAsync(string companyName)
    {
        // 1. Generate a safe schema name
        var schemaName = $"tenant_{companyName.ToLower().Replace(" ", "_")}";

        // 2. Create the physical schema in Postgres
        await context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA {schemaName};");

        // 3. Generate the SQL script from your DbContext
        var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
        string baseScript = databaseCreator.GenerateCreateScript();

        // 4. THE MAGIC: Replace placeholder schema with the NEW schemaName
        // Note: Postgres uses double quotes for schema names in generated scripts
        string tenantScript = baseScript.Replace("\"tenant\".", $"\"{schemaName}\".");

        // 5. Execute the script to build tables
        await context.Database.ExecuteSqlRawAsync(tenantScript);

        // 6. Register the record in the public 'tenants' table
        var newTenant = new Tenant.Domain.Models.Tenant
        {
            CompanyName = companyName,
            SchemaName = schemaName,
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };

        context.Tenants.Add(newTenant);
        await context.SaveChangesAsync();
    }
}
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Infrastructure;
// using Microsoft.EntityFrameworkCore.Storage;
// using Tenant.Api.Features.Tenants.Commands;
// using Tenant.Infrastructure.Data;
// using TenantClass = Tenant.Domain.Models.Tenant;

// namespace Tenant.Application.Features.Tenants.Handlers;

// public class CreateTenantHandler(TenantDbContext context) : IRequestHandler<CreateTenantCommand, string>
// {
//     public async Task<string> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
//     {
//         var schemaName = $"tenant_{request.CompanyName.ToLower().Replace(" ", "_")}";

//         // 1. Create the schema
//         await context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";");

//         // 2. Generate the full script
//         var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
//         string baseScript = databaseCreator.GenerateCreateScript();

//         // 3. THE FIX: Replace placeholder "tenant" with our new schema name
//         // We replace the schema name and the sequence references
//         string tenantScript = baseScript
//             .Replace("\"tenant\".", $"\"{schemaName}\".")
//             .Replace("'tenant.", $"'{schemaName}.");

//         // 4. Wrap the script in a 'Continue on Error' block for Postgres
//         // This allows the script to skip the 'public' tables that already exist
//         var finalScript = $@"
//         DO $$ 
//         BEGIN 
//             {tenantScript}
//         EXCEPTION WHEN others THEN 
//             RAISE NOTICE 'Skipping existing objects...';
//         END $$;";

//         // 5. Execute
//         await context.Database.ExecuteSqlRawAsync(finalScript);

//         // 6. Register master record
//         var newTenant = new TenantClass
//         {
//             CompanyName = request.CompanyName,
//             SchemaName = schemaName,
//             IsActive = true,
//             CreatedOn = DateTime.Now
//         };

//         context.Tenants.Add(newTenant);
//         await context.SaveChangesAsync(cancellationToken);

//         return schemaName;
//     }
// }
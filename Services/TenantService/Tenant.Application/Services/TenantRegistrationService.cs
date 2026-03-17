using Tenant.Domain.Models;
using TenantModel = Tenant.Domain.Models.Tenant;
using Tenant.Domain.Dtos;
using Tenant.Application.Interface;
using Tenant.Infrastructure.Interfaces;

namespace Tenant.Application.Services;

public class TenantRegistrationService(
    ITenantUnitOfWork unitOfWork) : ITenantRegistrationService
{
    public bool CreateTenantAsync(CreateTenantDto request)
    {
        // 4. Create Admin User
        // 1. Prepare the Tenant object
        var newTenant = new TenantModel
        {
            CompanyName = request.CompanyName,
            SchemaName = request.SchemaName,
            IsActive = true,
            CreatedOn = DateTime.Now // Best practice to use UtcNow
        };

        // 2. Add and Save Tenant to get the Identity ID
        unitOfWork.Tenants.Add(newTenant);
        unitOfWork.Save(); // The ID is now populated in newTenant.Id

        // 3. Now prepare the Admin User with the new TenantId
        var adminUser = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.AdminEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = 1,
            CreatedOn = DateTime.Now,
            TenantId = newTenant.TenantId // Use the ID generated in step 2
        };

        // 4. Save the User
        unitOfWork.Users.Add(adminUser);
        unitOfWork.Save();
        return true;
    }
}
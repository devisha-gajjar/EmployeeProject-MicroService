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
        // Create Tenant
        var newTenant = new TenantModel
        {
            CompanyName = request.CompanyName,
            SchemaName = request.SchemaName,
            IsActive = true,
            CreatedOn = DateTime.Now
        };

        unitOfWork.Tenants.Add(newTenant);
        unitOfWork.Save();

        // Create Admin User 
        var adminUser = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.AdminEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = 1,
            CreatedOn = DateTime.Now,
            TenantId = newTenant.TenantId
        };

        unitOfWork.Users.Add(adminUser);
        unitOfWork.Save();
        return true;
    }
}
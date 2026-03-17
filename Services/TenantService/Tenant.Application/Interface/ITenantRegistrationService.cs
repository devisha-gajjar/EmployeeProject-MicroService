using Tenant.Domain.Dtos;

namespace Tenant.Application.Interface;

public interface ITenantRegistrationService
{
    public bool CreateTenantAsync(CreateTenantDto request);
}

using Employee.Shared.Interfaces;
using Tenant.Domain.Models;
using TenantModel = Tenant.Domain.Models.Tenant;

namespace Tenant.Infrastructure.Interfaces;

public interface ITenantUnitOfWork : IUnitOfWork
{
    IGenericRepository<TenantModel> Tenants { get; }
    IGenericRepository<User> Users { get; }
}
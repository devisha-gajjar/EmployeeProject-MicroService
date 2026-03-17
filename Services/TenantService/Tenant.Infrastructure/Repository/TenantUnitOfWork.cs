using Employee.Shared.Interfaces;
using Employee.Shared.Repositories;
using Tenant.Domain.Models;
using Tenant.Infrastructure.Data.Host;
using Tenant.Infrastructure.Interfaces;
using TenantModel = Tenant.Domain.Models.Tenant;

namespace Tenant.Infrastructure.Repository;

public class TenantUnitOfWork : ITenantUnitOfWork
{
    private readonly HostDbContext _context;

    public IGenericRepository<TenantModel> Tenants { get; private set; }
    public IGenericRepository<User> Users { get; private set; }

    public TenantUnitOfWork(HostDbContext context)
    {
        _context = context;

        Tenants = new GenericRepository<TenantModel>(_context);
        Users = new GenericRepository<User>(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int Save()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
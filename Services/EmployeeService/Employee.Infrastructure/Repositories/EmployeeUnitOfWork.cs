using Employee.Application.Interfaces;
using Employee.Domain.Models;
using Employee.Infrastructure.Data;
using Employee.Shared.Interfaces;
using Employee.Shared.Repositories;

namespace Employee.Infrastructure.Repositories;

public class EmployeeUnitOfWork : IEmployeeUnitOfWork
{
    // Uses the specific DbContext for the Employee microservice
    private readonly Tenant1DbContext _context;

    public IGenericRepository<EmployeeList> Employees { get; private set; }
    public IGenericRepository<Department> Departments { get; private set; }

    public EmployeeUnitOfWork(Tenant1DbContext context)
    {
        _context = context;

        // Wire up the shared GenericRepository with the Employee database context
        Employees = new GenericRepository<EmployeeList>(_context);
        Departments = new GenericRepository<Department>(_context);
    }

    // Centralized save for the Employee transaction
    public int Save()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveAsync()
    {   
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
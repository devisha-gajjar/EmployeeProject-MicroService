using Employee.Domain.Models;
using Employee.Shared.Interfaces;

namespace Employee.Application.Interfaces;

// Inherits the Save() methods from the shared IUnitOfWork
public interface IEmployeeUnitOfWork : IUnitOfWork
{
    IGenericRepository<EmployeeList> Employees { get; }
    IGenericRepository<Department> Departments { get; }
    IGenericRepository<Tenant> Tenants { get; }
}
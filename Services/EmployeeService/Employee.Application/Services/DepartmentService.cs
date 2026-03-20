using Employee.Application.Interfaces;
using Employee.Application.ServiceInterfaces;
using Employee.Domain.Models;
using Employee.Shared.Exceptions;

namespace Employee.Application.Services;

public class DepartmentService(IEmployeeUnitOfWork employeeUnitOfWork) : IDepartmentService
{

    public IEnumerable<Department> GetDepartments()
    {
        return employeeUnitOfWork.Departments.GetAll();
    }

    public Department GetDepartmentById(int id)
    {
        return employeeUnitOfWork.Departments.GetById(id) ?? throw new AppException("Department not found!");
    }
}
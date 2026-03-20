using Employee.Domain.Models;

namespace Employee.Application.ServiceInterfaces;

public interface IDepartmentService
{
    IEnumerable<Department> GetDepartments();
    Department GetDepartmentById(int id);
}

using Employee.Domain.DTOs;
using Employee.Domain.Models;

namespace Employee.Application.Interfaces;

public interface IEmployeeService
{
    IEnumerable<EmployeeListDto> GetEmployees();

    AddEmployeeViewModelDto? GetEmployeeById(int id);

    Task<EmployeeList?> SaveEmployee(AddEmployeeViewModelDto employeeDto);

    bool DeleteEmployee(int id);

    Task<MemoryStream> ExportEmployees();
}

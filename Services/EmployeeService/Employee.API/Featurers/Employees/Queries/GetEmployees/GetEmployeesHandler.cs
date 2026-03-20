using Employee.Application.Interfaces;
using Employee.Domain.DTOs;

namespace Employee.API.Featurers.Employees.Queries.GetEmployees;
public class GetEmployeesHandler(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    public IEnumerable<EmployeeListDto> Handle()
    {
        return _employeeService.GetEmployees();
    }
}
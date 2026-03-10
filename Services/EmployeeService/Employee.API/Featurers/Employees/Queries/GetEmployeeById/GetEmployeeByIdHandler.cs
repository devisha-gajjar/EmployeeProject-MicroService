using Employee.Application.Interfaces;
using Employee.Domain.DTOs;

namespace Employee.API.Featurers.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdHandler(IEmployeeService employeeService)
{
    public AddEmployeeViewModelDto? Handle(int id) => employeeService.GetEmployeeById(id);
}
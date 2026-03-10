using Employee.Application.Interfaces;

namespace Employee.API.Featurers.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeHandler(IEmployeeService employeeService)
{
    public bool Handle(int id) => employeeService.DeleteEmployee(id);
}
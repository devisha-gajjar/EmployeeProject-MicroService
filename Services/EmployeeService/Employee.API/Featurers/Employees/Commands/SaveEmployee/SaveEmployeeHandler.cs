using System.Threading.Tasks;
using Employee.Application.Interfaces;
using Employee.Domain.DTOs;
using Employee.Domain.Models;

namespace Employee.API.Featurers.Employees.Commands.SaveEmployee;

public record SaveEmployeeCommand(
    int Id,
    string Name,
    string Email,
    int DepartmentId,
    decimal? Salary
);

    public class SaveEmployeeHandler(IEmployeeService employeeService)
    {
        public async Task<EmployeeList?> Handle(SaveEmployeeCommand command)
        {
            var dto = new AddEmployeeViewModelDto
            {
                Id = command.Id,
                Name = command.Name,
                Email = command.Email,
                DepartmentId = command.DepartmentId,
                Salary = command.Salary
            };

            return await employeeService.SaveEmployee(dto);
        }
    }

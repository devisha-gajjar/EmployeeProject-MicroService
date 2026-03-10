using Employee.Application.Interfaces;

namespace Employee.API.Featurers.Employees.Queries.ExportEmployee;

public class ExportEmployeesHandler(IEmployeeService employeeService)
{
    public async Task<MemoryStream> Handle() => await employeeService.ExportEmployees();
}
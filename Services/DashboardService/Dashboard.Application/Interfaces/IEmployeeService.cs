using Dashboard.Domain.DTOs;

namespace Dashboard.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDashboardDTO> GetEmployeeDashboardDataAsync();
}

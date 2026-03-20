namespace Employee.API.Extensions;

using Employee.API.Featurers.Employees.Commands.DeleteEmployee;
using Employee.API.Featurers.Employees.Commands.SaveEmployee;
using Employee.API.Featurers.Employees.Queries.ExportEmployee;
using Employee.API.Featurers.Employees.Queries.GetEmployeeById;
using Employee.API.Featurers.Employees.Queries.GetEmployees;
using Employee.Application.Interfaces;
using Employee.Application.Mappings;
using Employee.Application.ServiceInterfaces;
using Employee.Application.Services;
using Employee.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtentions
{
    public static IServiceCollection ServiceClass(
     this IServiceCollection services,
     IConfiguration configuration)
    {
        // Handlers
        services.AddScoped<GetEmployeesHandler>();
        services.AddScoped<GetEmployeeByIdHandler>();
        services.AddScoped<SaveEmployeeHandler>();
        services.AddScoped<DeleteEmployeeHandler>();
        services.AddScoped<ExportEmployeesHandler>();

        // Application Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        // Unit of Work
        services.AddScoped<IEmployeeUnitOfWork, EmployeeUnitOfWork>();

        // Automapper
        services.AddAutoMapper(typeof(EmployeeProfile).Assembly);

        return services;
    }
}

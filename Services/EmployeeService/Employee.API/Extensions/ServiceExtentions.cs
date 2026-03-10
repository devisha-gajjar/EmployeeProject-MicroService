namespace Employee.API.Extensions;

using Employee.API.Featurers.Employees.Commands.DeleteEmployee;
using Employee.API.Featurers.Employees.Commands.SaveEmployee;
using Employee.API.Featurers.Employees.Queries.ExportEmployee;
using Employee.API.Featurers.Employees.Queries.GetEmployeeById;
using Employee.API.Featurers.Employees.Queries.GetEmployees;
using Employee.Application.Interfaces;
using Employee.Application.Mappings;
using Employee.Application.Services;
using Employee.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtentions
{
    public static IServiceCollection ServiceClass(
     this IServiceCollection services,
     IConfiguration configuration)
    {
        // 1. Register Handlers
        services.AddScoped<GetEmployeesHandler>();
        services.AddScoped<GetEmployeeByIdHandler>();
        services.AddScoped<SaveEmployeeHandler>();
        services.AddScoped<DeleteEmployeeHandler>();
        services.AddScoped<ExportEmployeesHandler>();

        // 2. Register Application Services
        services.AddScoped<IEmployeeService, EmployeeService>();

        // 3. MISSING PART: Register the Unit of Work
        // Replace 'EmployeeUnitOfWork' with the actual name of your implementation class
        services.AddScoped<IEmployeeUnitOfWork, EmployeeUnitOfWork>();

        // 4. Also register any Repositories if the UnitOfWork doesn't handle them internally
        // services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddAutoMapper(typeof(EmployeeProfile).Assembly);

        return services;
    }
}

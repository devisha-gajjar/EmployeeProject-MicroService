using Employee.API.Featurers.Employees.Commands.DeleteEmployee;
using Employee.API.Featurers.Employees.Commands.SaveEmployee;
using Employee.API.Featurers.Employees.Queries.ExportEmployee;
using Employee.API.Featurers.Employees.Queries.GetEmployeeById;
using Employee.API.Featurers.Employees.Queries.GetEmployees;
using Microsoft.AspNetCore.Mvc;

namespace Employee.API.Endpoints;

public static class EmployeeEndpoints 
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/employee");

        group.MapGet("/", ([FromServices] GetEmployeesHandler handler) =>
            Results.Ok(handler.Handle()));

        group.MapGet("/{id:int}", (int id, [FromServices] GetEmployeeByIdHandler handler) =>
        {
            var result = handler.Handle(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapPost("/save", (SaveEmployeeCommand command, [FromServices] SaveEmployeeHandler handler) =>
        {
            var result = handler.Handle(command);
            return Results.Ok(result);
        });

        group.MapDelete("/{id:int}", (int id, [FromServices] DeleteEmployeeHandler handler) =>
        {
            var success = handler.Handle(id);
            return success ? Results.NoContent() : Results.BadRequest();
        });

        group.MapGet("/export", async ([FromServices] ExportEmployeesHandler handler) =>
        {
            var stream = await handler.Handle();
            return Results.File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Employees.xlsx"
            );
        });
    }
}
using Employee.API.Featurers.Departments.GetDepartmentById;
using Employee.API.Featurers.Departments.GetDepartments;
using MediatR;

namespace Employee.API.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/departments");

        group.MapGet("/", async (IMediator mediator) =>
        {
            return await mediator.Send(new GetDepartmentsQuery());
        });

        group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
        {
            return await mediator.Send(new GetDepartmentByIdQuery(id));
        });
    }
}

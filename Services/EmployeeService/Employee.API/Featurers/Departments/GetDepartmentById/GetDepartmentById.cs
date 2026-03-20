using Employee.Application.ServiceInterfaces;
using Employee.Domain.Models;
using MediatR;

namespace Employee.API.Featurers.Departments.GetDepartmentById;

public record GetDepartmentByIdQuery(int Id) : IRequest<Department>;

public class GetDepartmentByIdHandler(IDepartmentService departmentService) : IRequestHandler<GetDepartmentByIdQuery, Department>
{
    private readonly IDepartmentService _departmentService = departmentService;

    public Task<Department> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var result = _departmentService.GetDepartmentById(request.Id);
        return Task.FromResult(result);
    }
}
using Employee.Application.ServiceInterfaces;
using Employee.Domain.Models;
using MediatR;

namespace Employee.API.Featurers.Departments.GetDepartments;

public record GetDepartmentsQuery() : IRequest<IEnumerable<Department>>;

public class GetDepartmentsHandler(IDepartmentService departmentService) : IRequestHandler<GetDepartmentsQuery, IEnumerable<Department>>
{
    private readonly IDepartmentService _departmentService = departmentService;

    public Task<IEnumerable<Department>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var result = _departmentService.GetDepartments();
        return Task.FromResult(result);
    }
}
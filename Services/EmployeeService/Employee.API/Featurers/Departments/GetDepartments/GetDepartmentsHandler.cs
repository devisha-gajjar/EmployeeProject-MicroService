using Employee.Application.ServiceInterfaces;
using Employee.Domain.Models;
using MediatR;

namespace Employee.API.Featurers.Departments.GetDepartments;

public record GetDepartmentsQuery() : IRequest<IEnumerable<Department>>;

public class GetDepartmentsHandler(IDepartmentService departmentService) : IRequestHandler<GetDepartmentsQuery, IEnumerable<Department>>
{

    public Task<IEnumerable<Department>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var result = departmentService.GetDepartments();
        return Task.FromResult(result);
    }
}
namespace Employee.Application.Services;
using Employee.API.Protos;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Employee.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

public class EmployeeGrpcService(IEmployeeUnitOfWork repository) : EmployeeGrpc.EmployeeGrpcBase
{
    public override async Task<EmployeeDetailResponse> GetEmployeeDetails(EmployeeRequest request, ServerCallContext context)
    {
        var employee = await repository.Employees.GetByInclude(
                                    e => e.Id == int.Parse(request.Id),
                                    query => query.Include(e => e.Department)
                                ) ?? throw new RpcException(new Status(StatusCode.NotFound, "Employee not found"));

        var response = new EmployeeDetailResponse
        {
            FullName = $"{employee.Name}",
            Position = employee.Department.Name ?? "-",
            StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(
         employee.CreatedOn ?? DateTime.MinValue,
         DateTimeKind.Utc))
        };

        return response;
    }
}
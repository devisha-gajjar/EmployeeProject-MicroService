namespace Employee.Application.Services;

using Employee.API.Protos; // Ensure this matches the 'option csharp_namespace' in your proto
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Employee.Application.Interfaces; // Where your UnitOfWork lives
using Microsoft.EntityFrameworkCore;

// 1. Inherit from the generated Base class

public class EmployeeGrpcService(IEmployeeUnitOfWork repository) : EmployeeGrpc.EmployeeGrpcBase
{
    // 2. The return type MUST be EmployeeDetailResponse
    public override async Task<EmployeeDetailResponse> GetEmployeeDetails(EmployeeRequest request, ServerCallContext context)
    {
        var employee = await repository.Employees.GetByInclude(
                                    e => e.Id == int.Parse(request.Id),
                                    query => query.Include(e => e.Department) // This is where the magic happens
                                ) ?? throw new RpcException(new Status(StatusCode.NotFound, "Employee not found"));

        // 3. Create the specific Response object defined in your proto

        var response = new EmployeeDetailResponse
        {
            FullName = $"{employee.Name}",
            Position = employee.Department.Name ?? "-",
            // Use .Value and provide a fallback for the null case
            StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(
         employee.CreatedOn ?? DateTime.MinValue,
         DateTimeKind.Utc))
        };

        // foreach (var task in employee.Tasks)
        // {
        //     response.Tasks.Add(new TaskInfo { Name = task.Title, Status = task.Status });
        // }

        return response;
    }
}
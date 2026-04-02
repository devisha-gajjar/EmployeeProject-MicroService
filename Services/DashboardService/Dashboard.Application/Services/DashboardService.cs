using Dashboard.Domain.DTOs;

namespace Dashboard.API.Services;

public override async Task<EmployeeDashboardDTO> GetEmployeeDashboard(DashboardRequest request, ServerCallContext context)
{
    // 1. Call the Employee Microservice (Internal gRPC call)
    // '_employeeGrpcClient' is the client generated from your .proto
    var employeeData = await _employeeGrpcClient.GetEmployeeDetailsAsync(new EmployeeRequest { Id = request.EmployeeId });

    // 2. Map Protobuf Object -> Your DTO
    // This keeps your logic in "C# terms" (DateTime, List, etc.)
    var dashboardDto = new EmployeeDashboardDTO
    {
        FullName = employeeData.FullName,
        Position = employeeData.Position,
        EmploymentStartDate = employeeData.StartDate.ToDateTime(), // Converting Proto Timestamp to C# DateTime
        // Perform calculations here if needed
        TotalTasksAssigned = employeeData.Tasks.Count,
        PendingTasks = employeeData.Tasks.Count(t => t.Status == "Pending")
    };

    // 3. Map DTO -> Final gRPC Response
    var response = new EmployeeDashboardDTO
    {
        FullName = dashboardDto.FullName,
        Position = dashboardDto.Position,
        TotalTasksAssigned = dashboardDto.TotalTasksAssigned
    };

    // Add nested lists from the DTO
    foreach (var task in dashboardDto.Tasks)
    {
        response.Tasks.Add(new TaskDetail { Name = task.TaskName, Status = task.Status });
    }

    return response;
}
namespace Employee.Domain.DTOs;

public class EmployeeDashboardDTO
{
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime? EmploymentStartDate { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int TotalTasksAssigned { get; set; }
    public int PendingTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int TotalLeaveDays { get; set; }
    public int TotalPresentDays { get; set; }
    public int TotalAbsentDays { get; set; }
    public List<LeaveRequestDTO> LeaveRequests { get; set; } = new List<LeaveRequestDTO>();
    public List<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();
    public DepartmentDTO Department { get; set; }
}

public class LeaveRequestDTO
{
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TaskDTO
{
    public string TaskName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
}

public class DepartmentDTO
{
    public string DepartmentName { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
}


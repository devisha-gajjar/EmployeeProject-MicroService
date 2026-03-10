namespace Employee.Domain.DTOs;

public class AddEmployeeViewModelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public decimal? Salary { get; set; }
    public DateTime CreatedOn { get; set; }
}

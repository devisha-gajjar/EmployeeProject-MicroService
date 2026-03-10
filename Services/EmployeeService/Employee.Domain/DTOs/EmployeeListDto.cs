namespace Employee.Domain.DTOs;

public class EmployeeListDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string DepartmentName { get; set; }
    public int DepartmentId { get; set; }
    public decimal? Salary { get; set; }
    public DateTime? CreatedOn { get; set; }
}


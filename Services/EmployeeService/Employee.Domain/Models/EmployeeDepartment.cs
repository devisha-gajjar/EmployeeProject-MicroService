using System;
using System.Collections.Generic;

namespace Employee.Domain.Models;

public partial class EmployeeDepartment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? CurrentStatus { get; set; }

    public virtual Department? Department { get; set; }
}

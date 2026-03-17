using System;
using System.Collections.Generic;

namespace Tenant.Domain.Models;

public partial class EmployeeList
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public int DepartmentId { get; set; }

    public decimal? Salary { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual Department Department { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Tenant.Domain.Models;

public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ManagerId { get; set; }

    public virtual ICollection<EmployeeList> EmployeeLists { get; set; } = new List<EmployeeList>();

    public virtual User? Manager { get; set; }
}

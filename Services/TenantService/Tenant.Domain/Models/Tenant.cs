using System;
using System.Collections.Generic;

namespace Tenant.Domain.Models;

public partial class Tenant
{
    public int TenantId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string SchemaName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

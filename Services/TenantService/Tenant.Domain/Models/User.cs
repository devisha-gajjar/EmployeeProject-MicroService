using System;
using System.Collections.Generic;

namespace Tenant.Domain.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public string? ProfilePicture { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? EmploymentStartDate { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Position { get; set; }

    public bool IsTwoFactorEnabled { get; set; }

    public string? TwoFactorSecret { get; set; }

    public DateTime? TwoFactorEnabledOn { get; set; }

    public int FailedLoginCount { get; set; }

    public DateTime? LastFailedLogin { get; set; }

    public DateTime? LockoutUntil { get; set; }

    public int TenantId { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual Role Role { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}

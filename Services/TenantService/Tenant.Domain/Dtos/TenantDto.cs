namespace Tenant.Domain.Dtos;

public class TenantDto
{
    public int TenantId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string SchemaName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public TenantUserDto? AdminUser { get; set; }
    public List<TenantUserDto> Users { get; set; } = [];
}

public class TenantUserDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
}
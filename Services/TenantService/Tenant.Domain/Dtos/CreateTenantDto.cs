namespace Tenant.Domain.Dtos;

public class CreateTenantDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
}

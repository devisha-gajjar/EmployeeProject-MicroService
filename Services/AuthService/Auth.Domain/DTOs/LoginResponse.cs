using static Employee.Shared.Enums.Enum;

namespace Auth.Domain.DTOs;
public class LoginResponse
{
    public LoginStep Step { get; set; }
    public int FailedLoginAttempt { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? TempToken { get; set; }
    public string? RefreshToken { get; set; }
}

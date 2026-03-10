namespace Auth.Domain.DTOs;
public class Verify2FADto
{
    public string TempToken { get; set; } = null!;
    public string Code { get; set; } = null!;
}

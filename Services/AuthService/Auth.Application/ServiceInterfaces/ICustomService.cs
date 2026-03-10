using System.Security.Claims;

namespace Auth.Application.ServiceInterfaces;

public interface ICustomService
{
    public string Hash(string password);
    public bool Verify(string password, string hashedPassword);
    public string GenerateJwtToken(string name);
    public string GenerateTempToken(int userId, bool rememberMe);
    ClaimsPrincipal? ValidateTempToken(string token);
}

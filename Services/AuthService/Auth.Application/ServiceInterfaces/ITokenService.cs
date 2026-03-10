using System.Security.Claims;
using Auth.Domain.Models;

namespace Auth.Application.ServiceInterfaces;

public interface ITokenService
{
    public string GenerateAccessToken(User? user);
    public string GenerateRefreshToken(User user, bool rememberMe);
    public ClaimsPrincipal ValidateToken(string token, bool validateLifetime = true);
    public bool IsRememberMeEnabled(ClaimsPrincipal principal);
    public string GetUserIdFromToken(ClaimsPrincipal principal);
    public DateTime GetTokenExpiration(string token);
}

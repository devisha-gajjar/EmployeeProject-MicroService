using Auth.Domain.DTOs;
using Auth.Domain.Models;

namespace Auth.Application.ServiceInterfaces;

public interface IAuthService
{
    public User? Register(User user, string password);
    Task<LoginResponse> Login(UserLoginDto dto);
    Task<AuthTokenResponseDto> VerifyTwoFactorAsync(Verify2FADto dto);
    Task<(string accessToken, string refreshToken)> ValidateRefreshTokens(string refreshToken);
    Task<(string accessToken, string refereshToken)> AuthenticateUser(UserLoginDto userLoginDto);
}

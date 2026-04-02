using System.Security.Claims;
using Auth.Application.ServiceInterfaces;
using Auth.Domain.Constants;
using Auth.Domain.DTOs;
using Auth.Domain.Models;

namespace Auth.API.Endpoints;

public static class AuthHandlers
{
    #region  Register
    public static IResult Register(RegisterDto dto, IAuthService authService)
    {
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            Zipcode = dto.Zipcode,
            RoleId = dto.RoleId
        };

        var result = authService.Register(user, dto.Password);

        if (result == null)
            return Results.BadRequest("User already exists");

        return Results.Ok(new { Message = "Registration successful" });
    }
    #endregion

    #region Login
    public static async Task<IResult> Login(
        UserLoginDto dto,
        IAuthService authService,
        HttpContext httpContext,
        IConfiguration configuration)
    {
        var result = await authService.Login(dto);

        if (result.RefreshToken != null)
        {
            var expiryTime = dto.RememberMe
                ? DateTime.UtcNow.AddDays(double.Parse(configuration["RememberMeExpiryDays"]!))
                : DateTime.UtcNow.AddHours(double.Parse(configuration["RefreshTokenExpiryHours"]!));

            httpContext.Response.Cookies.Append(Constants.REFRESH_TOKEN_KEY, result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiryTime
            });
        }

        return Results.Ok(result);
    }
    #endregion

    #region Refresh Token
    public static async Task<IResult> Refresh(
        HttpContext context,
        IAuthService authService)
    {
        var refreshToken = context.Request.Cookies[Constants.REFRESH_TOKEN_KEY];

        if (string.IsNullOrEmpty(refreshToken))
            return Results.BadRequest("Refresh token missing");

        var (newAccessToken, newRefreshToken) =
            await authService.ValidateRefreshTokens(refreshToken);

        context.Response.Cookies.Append(Constants.REFRESH_TOKEN_KEY, newRefreshToken);

        return Results.Ok(new
        {
            accessToken = newAccessToken
        });
    }
    #endregion

    #region Verify 2FA
    public static async Task<IResult> Verify2FA(
        Verify2FADto dto,
        IAuthService authService,
        HttpContext context)
    {
        var result = await authService.VerifyTwoFactorAsync(dto);

        context.Response.Cookies.Append(Constants.REFRESH_TOKEN_KEY, result.RefreshToken!);

        return Results.Ok(result);
    }
    #endregion

    #region Setup 2FA
    public static IResult Setup2FA(
        HttpContext context,
        ITwoFactorService twoFactorService)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Results.Unauthorized();

        int userId = int.Parse(userIdClaim);

        var result = twoFactorService.Generate2FASetup(userId);

        return Results.Ok(result);
    }
    #endregion
}
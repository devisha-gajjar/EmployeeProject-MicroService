using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Application.ServiceInterfaces;
using Auth.Domain.Constants;
using Auth.Domain.Models;
using Employee.Shared.Constants;
using Employee.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(User? user)
    {
        if (user == null)
        {
            throw new ArgumentException(GlobalConstants.USER_NOT_FOUND);
        }

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),    
                new Claim(ClaimTypes.Role, user.Role.RoleName!),
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Tenant.SchemaName),
                new Claim("Tenant Id", user.TenantId.ToString()),
                new Claim("2fa", "true")
            };

        var expiryMinutesString = configuration["AccessTokenExpiryMinutes"] ?? throw new InvalidOperationException(Constants.ACCESS_TOKEN_EXPIRYTIME_NOT_CONFIGURED_MESSAGE);
        return CreateToken(claims, DateTime.UtcNow.AddMinutes(double.Parse(expiryMinutesString)));
    }

    public string GenerateRefreshToken(User user, bool rememberMe)
    {
        var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserId.ToString()),
                new (Constants.REMEMBER_ME_CLAIM_NAME, rememberMe.ToString())
            };
        var expiryTime = rememberMe
                ? DateTime.UtcNow.AddDays(double.Parse(configuration["RememberMeExpiryDays"] ?? throw new AppException(Constants.REFRESH_TOKEN_EXPIRYTIME_NOT_CONFIGURED_MESSAGE)))
                : DateTime.UtcNow.AddHours(double.Parse(configuration["RefreshTokenExpiryHours"] ?? throw new AppException(Constants.REFRESH_TOKEN_EXPIRYTIME_NOT_CONFIGURED_MESSAGE)));

        return CreateToken(claims, expiryTime);
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expires)
    {
        var keyString = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException(Constants.JWT_KEY_ERROR_MESSAGE);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token, bool validateLifetime = true)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(Constants.EMPTY_TOKEN_MESSAGE);
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException(Constants.JWT_KEY_ERROR_MESSAGE)
        );

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AppException(Constants.INVALID_TOKEN_FORMAT_MESSAGE);
            }

            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new AppException(Constants.EXPIRED_TOKEN_MESSAGE, StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex) when (
            ex is SecurityTokenException ||
            ex is ArgumentException ||
            ex is FormatException
        )
        {
            throw new AppException(Constants.INVALID_TOKEN_FORMAT_MESSAGE, StatusCodes.Status401Unauthorized);
        }

        catch (Exception ex)
        {
            throw new AppException(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public bool IsRememberMeEnabled(ClaimsPrincipal principal)
    {
        var rememberMeClaim = principal.FindFirst(Constants.REMEMBER_ME_CLAIM_NAME)?.Value;
        return bool.TryParse(rememberMeClaim, out var rememberMe) && rememberMe;
    }

    public string GetUserIdFromToken(ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    public DateTime GetTokenExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.ValidTo;
    }
}
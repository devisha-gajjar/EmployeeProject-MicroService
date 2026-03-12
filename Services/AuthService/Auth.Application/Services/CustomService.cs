using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Application.ServiceInterfaces;
using Auth.Domain.Constantss;
using Auth.Domain.Models;
using Employee.Shared.Constants;
using Employee.Shared.Exceptions;
using Employee.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class CustomService(IGenericRepository<User> userRepository, IConfiguration config) : ICustomService
{
    #region PasswordHash
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    #endregion

    #region Token
    public string GenerateJwtToken(string name)
    {
        User user = userRepository.GetAll().Include(u => u.Role).FirstOrDefault(u => u.Username == name) ?? throw new AppException(GlobalConstants.UNAUTHORIZED_USER);

        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new AppException(Constants.JWT_KEY_ERROR_MESSAGE);

        byte[] key = Encoding.UTF8.GetBytes(jwtSecret);

        Claim[] authClaims =
        [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName!),
            new Claim(ClaimTypes.Name, user.UserId.ToString()),
            new Claim(ClaimTypes.GivenName, user.Username),
            new Claim("2fa", "true")
        ];

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        JwtSecurityToken token = new(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: authClaims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateTempToken(int userId, bool rememberMe)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
           ?? throw new AppException("JWT_SECRET not configured");

        byte[] key = Encoding.UTF8.GetBytes(jwtSecret);

        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("type", "2fa"),
            new Claim(ClaimTypes.UserData,rememberMe.ToString()),
        };

        var credentials = new SigningCredentials(
                 new SymmetricSecurityKey(key),
                 SecurityAlgorithms.HmacSha256
             );

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials
        );

        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateTempToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
           ?? throw new AppException("JWT_SECRET not configured");

        byte[] key = Encoding.UTF8.GetBytes(jwtSecret);

        try
        {
            var principal = tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero
                },
                out _
            );

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
    #endregion
}

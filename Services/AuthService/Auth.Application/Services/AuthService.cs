using System.Security.Claims;
using Auth.Application.ServiceInterfaces;
using Auth.Domain.Constantss;
using Auth.Domain.DTOs;
using Auth.Domain.Models;
using Auth.Infrastructure.Interface;
using Employee.Shared.Constants;
using Employee.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Employee.Shared.Enums.Enum;

namespace Auth.Application.Services;

public class AuthService(ICustomService customService, IGenericRepository<User> userRepository, ITwoFactorService twoFactorService, ITokenService tokenService, ILogger<AuthService> logger) : IAuthService
{
    #region Register
    public User? Register(User user, string password)
    {
        var existingUser = userRepository.GetByInclude(
                u => u.Email == user.Email || u.Username == user.Username
            );

        if (existingUser != null)
        {
            return null;
        }

        user.RoleId = 2;
        user.Password = customService.Hash(password);
        user.CreatedOn = DateTime.Now;
        user.IsDeleted = false;

        userRepository.Add(user);

        return userRepository.GetById(user.UserId);
    }
    #endregion

    #region Login
    public async Task<LoginResponse> Login(UserLoginDto dto)
    {
        var user = await userRepository.GetByInclude(
            u => (u.Email == dto.Email || u.Username == dto.Email) && !u.IsDeleted,
            q => q.Include(u => u.Role)
        ) ?? throw new AppException(Constants.INVALID_LOGIN_CREDENTIALS_MESSAGE);

        if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.Now)
            throw new AppException("Account is temporarily locked. Try again later.");

        if (user.FailedLoginCount >= 3)
        {
            if (string.IsNullOrWhiteSpace(dto.CaptchaToken))
                throw new AppException("Captcha required.");

            var captchaValid = await VerifyCaptchaAsync(dto.CaptchaToken);
            if (!captchaValid)
                throw new AppException("Invalid captcha.");
        }

        if (!customService.Verify(dto.Password!, user.Password))
        {
            user.FailedLoginCount++;
            user.LastFailedLogin = DateTime.Now;

            // Optional: lock account after 10 failures
            if (user.FailedLoginCount >= 10)
                user.LockoutUntil = DateTime.Now.AddMinutes(15);

            userRepository.Update(user);

            // Return the failed login count
            return new LoginResponse
            {
                FailedLoginAttempt = user.FailedLoginCount,
                Message = "Invalid credentials"
            };
        }

        // Successful login → reset counters
        user.FailedLoginCount = 0;
        user.LastFailedLogin = null;
        user.LockoutUntil = null;

        userRepository.Update(user);

        // 2FA enabled
        if (user.IsTwoFactorEnabled)
        {
            return new LoginResponse
            {
                Step = string.IsNullOrWhiteSpace(user.TwoFactorSecret)
                    ? LoginStep.RequireTwoFactorSetup
                    : LoginStep.RequireTwoFactor,
                TempToken = customService.GenerateTempToken(user.UserId, dto.RememberMe),
                FailedLoginAttempt = user.FailedLoginCount // Include the failed login count in the response
            };
        }
        logger.LogInformation("User logged in successfully");

        // No 2FA → issue tokens
        return IssueTokens(user, dto.RememberMe);
    }

    private LoginResponse IssueTokens(User user, bool rememberMe)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user, rememberMe);

        return new LoginResponse
        {
            Step = LoginStep.Success,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<(string accessToken, string refereshToken)> AuthenticateUser(UserLoginDto userLoginDto)
    {
        if (userLoginDto == null || string.IsNullOrEmpty(userLoginDto.Email) || string.IsNullOrEmpty(userLoginDto.Password))
        {
            throw new ArgumentException(Constants.INVALID_LOGIN_CREDENTIALS_MESSAGE);
        }

        User? user = await userRepository.GetByInclude(u => u.Email.ToLower() == userLoginDto.Email.ToLower() && !u.IsDeleted,
                                                            query => query.Include(u => u.Role)) ?? throw new ArgumentException(GlobalConstants.USER_NOT_FOUND);


        if (!customService.Verify(userLoginDto.Password, user.Password))
        {
            throw new AppException(Constants.INVALID_LOGIN_CREDENTIALS_MESSAGE);
        }

        string accessToken = tokenService.GenerateAccessToken(user);
        string refreshToken = tokenService.GenerateRefreshToken(user, userLoginDto.RememberMe);
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            throw new Exception(Constants.FAILED_TOKEN_GENERATION_MESSAGE);
        }

        return (accessToken, refreshToken);
    }

    #endregion

    #region Verify 2FA token
    public async Task<AuthTokenResponseDto> VerifyTwoFactorAsync(Verify2FADto dto)
    {
        // 1. Validate temporary JWT (issued after username/password)
        var principal = customService.ValidateTempToken(dto.TempToken)
            ?? throw new AppException("Invalid or expired temporary token");

        var userId = int.Parse(
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new AppException("Invalid token payload")
        );

        // 2. Load user
        var user = userRepository.GetById(userId)
            ?? throw new AppException("User not found");

        if (string.IsNullOrWhiteSpace(user.TwoFactorSecret))
            throw new AppException("2FA is not enabled for this user");

        // 3. Validate TOTP code
        var isValid = twoFactorService.ValidateCode(
            user.TwoFactorSecret,
            dto.Code
        );

        var rememberMe = bool.Parse(principal.FindFirst(ClaimTypes.UserData)?.Value!);
        if (!isValid)
            throw new AppException("Invalid authentication code");

        // 4. Issue final access token
        var accessToken = customService.GenerateJwtToken(user.Username);
        var refreshToken = tokenService.GenerateRefreshToken(user, rememberMe);

        return new AuthTokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RememberMe = rememberMe
        };
    }
    #endregion

    #region Validate Refresh Token
    public async Task<(string accessToken, string refreshToken)> ValidateRefreshTokens(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentException(Constants.REFRESH_TOKEN_REQUIRED_MESSAGE);
        }

        ClaimsPrincipal principal;
        bool isExpired = false;

        try
        {
            principal = tokenService.ValidateToken(refreshToken, validateLifetime: true);
        }
        catch (AppException ex) when (ex.StatusCode == StatusCodes.Status401Unauthorized && ex.Message.Contains("expired"))
        {
            principal = tokenService.ValidateToken(refreshToken, validateLifetime: false);
            isExpired = true;
        }

        if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
        {
            throw new ArgumentException(Constants.INVALID_DATA_MESSAGE);
        }

        var userIdStr = tokenService.GetUserIdFromToken(principal);
        if (!int.TryParse(userIdStr, out int userId))
        {
            throw new ArgumentException(Constants.INVALID_USER_ID_MESSAGE);
        }

        User? user = await userRepository.GetByInclude(u => u.UserId == userId && !u.IsDeleted,
            query => query.Include(u => u.Role))
            ?? throw new ArgumentException(GlobalConstants.USER_NOT_FOUND);

        if (isExpired)
        {
            throw new AppException(Constants.EXPIRED_LOGIN_SESSION_MESSAGE, StatusCodes.Status401Unauthorized);
        }

        string newAccessToken = tokenService.GenerateAccessToken(user);
        string newRefreshToken = tokenService.GenerateRefreshToken(user, tokenService.IsRememberMeEnabled(principal));

        if (string.IsNullOrEmpty(newAccessToken) || string.IsNullOrEmpty(newRefreshToken))
        {
            throw new Exception(Constants.FAILED_TOKEN_GENERATION_MESSAGE);
        }

        return (newAccessToken, newRefreshToken);
    }
    #endregion

    #region Verify Captcha
    private static async Task<bool> VerifyCaptchaAsync(string token)
    {
        var secretKey = Environment.GetEnvironmentVariable("CLOUDFLARE_SECRET");

        var values = new Dictionary<string, string>
                {
                    { "secret", secretKey! },
                    { "response", token }
                };

        using var client = new HttpClient();
        var response = await client.PostAsync(
            "https://challenges.cloudflare.com/turnstile/v0/siteverify",
            new FormUrlEncodedContent(values)
        );

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(json) ?? throw new AppException("Invalid Captcha");

        return result.success == true;
    }

    #endregion
}

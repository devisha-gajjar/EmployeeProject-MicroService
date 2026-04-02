namespace Auth.Domain.Constants;

public static class Constants
{
    #region Keys
    public const string REFRESH_TOKEN_KEY = "refreshToken";
    #endregion

    #region Token Messages
    public const string REMEMBER_ME_CLAIM_NAME = "Remeber me";
    public const string INVALID_TOKEN_FORMAT_MESSAGE = "Invalid Token Format.";
    public const string EXPIRED_TOKEN_MESSAGE = "Token has expired.";
    public const string EMPTY_TOKEN_MESSAGE = "Token must not be null or empty.";
    public const string JWT_KEY_ERROR_MESSAGE = "JWT Key is not configured.";
    public const string ACCESS_TOKEN_EXPIRYTIME_NOT_CONFIGURED_MESSAGE = "Access token expiry time is not set";
    public const string REFRESH_TOKEN_EXPIRYTIME_NOT_CONFIGURED_MESSAGE = "Refresh token expiry time is not set";
    public const string REFRESH_TOKEN_REQUIRED_MESSAGE = "Refresh token is required.";
    public const string INVALID_DATA_MESSAGE = "Invalid Data.";
    public const string INVALID_USER_ID_MESSAGE = "Invalid UserId.";
    public const string EXPIRED_LOGIN_SESSION_MESSAGE = "Your Login Session has expired. Please login again.";
    public const string FAILED_TOKEN_GENERATION_MESSAGE = "Failed to generate session tokens.";
    public const string INVALID_LOGIN_CREDENTIALS_MESSAGE = "Invalid Login Credentials.";
    #endregion
}

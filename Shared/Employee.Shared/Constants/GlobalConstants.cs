namespace Employee.Shared.Constants;

public class GlobalConstants
{
    #region Common Msg
    public const string UNAUTHORIZED_USER = "User is not authorized.";
    public const string ADMIN_GROUP = "Admins";
    public const string BEARER = "Bearer";
    public const string USER_NOT_FOUND = "User not found!";
    #endregion

    #region Employee
    public const string EMP_NOT_FOUND = "Employee not found!";
    #endregion

    #region Email Service
    public const string EMAIL_NOT_SENT = "Email not sent.";
    public const string SMTP_CONFIG_MISSING = "SMTP configuration is missing required fields.";
    public const string EMAIL_BODY_EMPTY = "Email body is not provided.";
    public const string EMAIL_PATH_NOT_CONFIGURED = "Email template path is not configured.";
    public const string EMAIL_SENT_SUCCESS = "Email successfully sent to {0}.";
    #endregion

    #region RabbitMQ
    public const string RABBITMQ_CONFIG_MISSING = "RabbitMQ connection string not found.";
    #endregion
}

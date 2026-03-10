namespace Employee.Shared.Enums;

public class Enum
{
    public enum LoginStep
    {
        Success = 1,          // Access token issued
        RequireTwoFactor = 2, // OTP verification required
        RequireTwoFactorSetup = 3 // Show QR / setup screen
    }
}

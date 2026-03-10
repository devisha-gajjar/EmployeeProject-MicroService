
using Auth.Domain.DTOs;

namespace Auth.Application.ServiceInterfaces;

public interface ITwoFactorService
{
    public string GenerateSecret();
    public string GenerateQrUri(string email, string secret);
    public bool ValidateCode(string secret, string code);
    public TwoFactorSetupDto Generate2FASetup(int userId);
}

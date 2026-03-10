namespace Auth.Domain.DTOs;
public class TwoFactorSetupDto
{
    public string Secret { get; set; } = default!;
    public string QrCodeUri { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Account { get; set; } = default!;
}

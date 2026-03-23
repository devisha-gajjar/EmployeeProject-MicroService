using EmailService.DTOs;

namespace EmailService.Service;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequestDto emailRequestDto);
}

namespace EmailService.Service;

using System.Net;
using System.Net.Mail;
using Employee.Shared.Constants;
using Employee.Shared.Exceptions;
using global::EmailService.DTOs;
using Microsoft.Extensions.Configuration;

public class EmailService(IConfiguration config) : IEmailService
{
    public virtual async Task<bool> SendEmailAsync(EmailRequestDto emailRequestDto)
    {
        try
        {
            var from = config["EmailSettings:FromAddress"];
            var host = config["EmailSettings:SmtpHost"];
            var port = int.Parse(config["EmailSettings:SmtpPort"] ?? "587");
            var username = config["EmailSettings:SmtpUsername"];
            var password = Environment.GetEnvironmentVariable("EmailSettings__SmtpPassword");

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new AppException(GlobalConstants.SMTP_CONFIG_MISSING);

            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(from!),
                Subject = emailRequestDto.Subject,
                Body = await GetHtmlTemplateAsync(emailRequestDto.TemplateType, emailRequestDto.Data),
                IsBodyHtml = true
            };

            message.To.Add(emailRequestDto.To);

            // multiple CCs
            if (emailRequestDto.Cc != null)
            {
                foreach (var cc in emailRequestDto.Cc)
                    message.CC.Add(cc);
            }
            // mutliple Bccs
            if (emailRequestDto.Bcc != null)
            {
                foreach (var bcc in emailRequestDto.Bcc)
                    message.Bcc.Add(bcc);
            }

            await SendAsync(smtpClient, message);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected virtual Task SendAsync(SmtpClient client, MailMessage message)
    {
        return client.SendMailAsync(message);
    }

    private static async Task<string> GetHtmlTemplateAsync(string templateName, Dictionary<string, string> data)
    {
        // Path: wwwroot/templates/WelcomeEmail.html
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", $"{templateName}.html");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Template {templateName} not found at {filePath}");

        string templateContent = await File.ReadAllTextAsync(filePath);

        if (data != null)
        {
            foreach (var item in data)
            {
                templateContent = templateContent.Replace($"{{{{{item.Key}}}}}", item.Value);
            }
        }

        return templateContent;
    }
}

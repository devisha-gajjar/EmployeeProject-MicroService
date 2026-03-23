namespace EmailService.DTOs;

public class EmailRequestDto
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string TemplateType { get; set; }
    public required Dictionary<string, string> Data { get; set; }
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
}
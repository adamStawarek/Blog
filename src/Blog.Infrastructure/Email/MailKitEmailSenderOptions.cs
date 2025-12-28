namespace Blog.Infrastructure.Emaill;

public class MailKitEmailSenderOptions
{
    public const string Key = "Email";

    public string SmtpHost { get; set; } = null!;
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = null!;
    public string SmtpPassword { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;
    public string SenderName { get; set; } = null!;
}
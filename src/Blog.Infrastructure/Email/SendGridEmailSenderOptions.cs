namespace Blog.Infrastructure.Email;
public class SendGridEmailSenderOptions
{
    public const string Key = "Email";

    public string? SendGridKey { get; set; }
    public string SenderAddress { get; set; } = null!;
    public string SenderName { get; set; } = null!;
}
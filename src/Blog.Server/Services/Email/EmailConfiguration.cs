namespace Blog.Server.Services.Email;
public class EmailConfiguration
{
    public const string Key = "Email";

    public string? SendGridKey { get; set; }
    public string SenderAddress { get; set; } = null!;
    public string SenderName { get; set; } = null!;
}

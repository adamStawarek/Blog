using Blog.Application.Services.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Blog.Server.Services.Email;
public class SendGridEmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public SendGridEmailSender(
        IOptions<SendGridEmailSenderOptions> optionsAccessor,
        ILogger<SendGridEmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public SendGridEmailSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendAsync(string toEmail, string subject, string message)
    {
        ArgumentNullException.ThrowIfNull(Options.SendGridKey, nameof(Options.SendGridKey));
        await Execute(Options.SendGridKey, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(Options.SenderAddress, Options.SenderName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);

        _logger.LogInformation(response.IsSuccessStatusCode ?
            $"Email to {toEmail} queued successfully!" :
            $"Failure Email to {toEmail}");
    }
}
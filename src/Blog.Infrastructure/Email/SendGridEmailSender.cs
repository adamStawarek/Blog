using Blog.Application.Services.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Blog.Infrastructure.Email;
public class SendGridEmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly SendGridEmailSenderOptions _options;

    public SendGridEmailSender(
        IOptions<SendGridEmailSenderOptions> optionsAccessor,
        ILogger<SendGridEmailSender> logger)
    {
        _options = optionsAccessor.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_options.SendGridKey, nameof(_options.SendGridKey));

        var client = new SendGridClient(_options.SendGridKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_options.SenderAddress, _options.SenderName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg, cancellationToken);

        _logger.LogInformation(response.IsSuccessStatusCode ?
            $"Email to {toEmail} queued successfully!" :
            $"Failure Email to {toEmail}");
    }
}
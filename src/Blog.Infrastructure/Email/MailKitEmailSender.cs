using Blog.Application.Services.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Blog.Infrastructure.Emaill;
public sealed class MailKitEmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly MailKitEmailSenderOptions _options;

    public MailKitEmailSender(ILogger logger, IOptions<MailKitEmailSenderOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task SendAsync(string receiver, string subject, string message, CancellationToken cancellationToken = default)
    {
        var mailMessage = new MimeMessage
        {
            Subject = subject
        };

        mailMessage.From
            .Add(new MailboxAddress(name: _options.SenderName, address: _options.SenderAddress));

        mailMessage.To
            .Add(new MailboxAddress(name: receiver, address: receiver));

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message
        };

        mailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(host: _options.SmtpHost, port: _options.SmtpPort, useSsl: true);
        await client.AuthenticateAsync(userName: _options.SmtpUser, password: _options.SmtpPassword, cancellationToken: cancellationToken);

        await client.SendAsync(mailMessage);

        await client.DisconnectAsync(quit: true, cancellationToken: cancellationToken);

        _logger.LogInformation("Email with {subject} to {receiver} was sent.", subject, receiver);
    }
}
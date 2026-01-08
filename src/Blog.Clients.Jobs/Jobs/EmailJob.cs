using Blog.Application.Jobs;
using Blog.Application.Services.Email;

namespace Blog.Clients.Jobs.Jobs;

public class EmailJob : IEmailJob
{
    private readonly IEmailSender _emailSender;

    public EmailJob(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task ExecuteAsync(EmailJobParams args, CancellationToken cancellationToken = default)
    {
        await _emailSender.SendAsync(args.Email, args.Subject, args.Body, cancellationToken);
    }
}
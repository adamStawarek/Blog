namespace Blog.Application.Services.Email;

public interface IEmailSender
{
    Task SendAsync(string receiver, string subject, string message, CancellationToken cancellationToken = default);
}
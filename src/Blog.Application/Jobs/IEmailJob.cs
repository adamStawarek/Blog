using Blog.Application.Jobs.Base;

namespace Blog.Application.Jobs;

public interface IEmailJob : IBlogJob<EmailJobParams>
{
}

public class EmailJobParams : IBlogJobParams
{
    public required string Email { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}
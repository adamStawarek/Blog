namespace Blog.Application.Events.Base;
public interface IBlogJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
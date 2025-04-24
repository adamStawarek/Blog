namespace Blog.Application.Jobs.Base;
public interface IBlogJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
namespace Blog.Clients.Web.Jobs.Jobs;
public interface IBlogJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
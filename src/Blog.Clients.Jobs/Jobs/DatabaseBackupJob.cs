namespace Blog.Clients.Web.Jobs.Jobs;
public class DatabaseBackupJob : IDatabaseBackupJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("XX");
        return Task.CompletedTask;
    }
}
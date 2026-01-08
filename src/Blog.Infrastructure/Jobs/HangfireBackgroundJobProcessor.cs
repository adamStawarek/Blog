using Blog.Application.Jobs.Base;
using Blog.Application.Services.Jobs;
using Hangfire;

namespace Blog.Infrastructure.Jobs;

public class HangfireBackgroundJobProcessor : IBackgroundJobProcessor
{
    public string Enqueue<T, V>(V args) where T : IBlogJob<V> where V : IBlogJobParams
        => BackgroundJob.Enqueue<T>((x) => x.ExecuteAsync(args, CancellationToken.None));
    public string Enqueue<T>() where T : IBlogJob
        => BackgroundJob.Enqueue<T>((x) => x.ExecuteAsync(CancellationToken.None));
    public string Enqueue(Action action) => BackgroundJob.Enqueue(() => action());
    public string Enqueue(Func<Task> action) => BackgroundJob.Enqueue(() => action());
}
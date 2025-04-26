using Blog.Application.Services.Jobs;
using Hangfire;

namespace Blog.Infrastructure.Jobs;
public class HangfireBackgroundJobProcessor : IBackgroundJobProcessor
{
    public string Enqueue(Action action) => BackgroundJob.Enqueue(() => action());

    public string Enqueue(Func<Task> action) => BackgroundJob.Enqueue(() => action());
}
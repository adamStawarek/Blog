using Blog.Application.Events.Base;
using Blog.Clients.Web.Jobs.Jobs;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Blog.Clients.Jobs.Utils;
public partial class JobsHostedService : IHostedService
{
    private readonly JobsOptions _options;

    public JobsHostedService(IOptions<JobsOptions> options)
    {
        _options = options.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RegisterJob<IDatabaseBackupJob>();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void RegisterJob<TJob>() where TJob : IBlogJob
    {
        var jobConfig = _options.Jobs.Single(x => x.JobId == typeof(TJob).Name);

        if (jobConfig.Enabled)
        {
            RecurringJob.AddOrUpdate<TJob>(jobConfig.JobId, x => x.ExecuteAsync(default), jobConfig.Cron);
        }
        else
        {
            RecurringJob.RemoveIfExists(jobConfig.JobId);
        }
    }
}
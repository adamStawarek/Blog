using Blog.Application.Jobs.Base;
using Blog.Application.Services.Jobs;

namespace Blog.Clients.Web.Api.Tests.Integration.Mocks;
internal class BackgroundJobProcessorMock : IBackgroundJobProcessor
{
    public string Enqueue(Action action) => string.Empty;

    public string Enqueue(Func<Task> action) => string.Empty;

    public string Enqueue<T>() where T : IBlogJob => string.Empty;

    public string Enqueue<T, V>(V args)
        where T : IBlogJob<V>
        where V : IBlogJobParams => string.Empty;
}
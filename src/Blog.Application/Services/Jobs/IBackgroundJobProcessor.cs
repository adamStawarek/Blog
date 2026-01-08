using Blog.Application.Jobs.Base;

namespace Blog.Application.Services.Jobs;

public interface IBackgroundJobProcessor
{
    string Enqueue(Action action);
    string Enqueue(Func<Task> action);
    string Enqueue<T>() where T : IBlogJob;
    string Enqueue<T, V>(V args) where T : IBlogJob<V> where V : IBlogJobParams;
}
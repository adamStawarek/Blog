namespace Blog.Application.Services.Jobs;
public interface IBackgroundJobProcessor
{
    string Enqueue(Action actionn);
    string Enqueue(Func<Task> action);
}

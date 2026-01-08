namespace Blog.Application.Jobs.Base;

public interface IBlogJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IBlogJob<TParams> where TParams : IBlogJobParams
{
    Task ExecuteAsync(TParams args, CancellationToken cancellationToken = default);
}
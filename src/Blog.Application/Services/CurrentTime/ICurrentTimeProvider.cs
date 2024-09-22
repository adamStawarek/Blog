namespace Blog.Application.Services.CurrentTime;
public interface ICurrentTimeProvider
{
    DateTimeOffset Now { get; }
}
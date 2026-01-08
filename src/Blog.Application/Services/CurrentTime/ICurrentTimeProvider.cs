namespace Blog.Application.Services.CurrentTime;

public interface ICurrentTimeProvider
{
    Func<DateTimeOffset> Now { get; }
}
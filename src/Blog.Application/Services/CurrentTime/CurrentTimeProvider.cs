namespace Blog.Application.Services.CurrentTime;
public class CurrentTimeProvider : ICurrentTimeProvider
{
    public Func<DateTimeOffset> Now => () => DateTimeOffset.Now;
}
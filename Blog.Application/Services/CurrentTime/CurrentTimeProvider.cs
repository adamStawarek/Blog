namespace Blog.Application.Services.CurrentTime;
public class CurrentTimeProvider : ICurrentTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
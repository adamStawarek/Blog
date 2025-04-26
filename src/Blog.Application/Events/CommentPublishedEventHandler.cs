using Blog.Domain.Events;
using MediatR;

namespace Blog.Application.Events;
public class CommentPublishedEventHandler : INotificationHandler<CommentPublishedEvent>
{
    public Task Handle(CommentPublishedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
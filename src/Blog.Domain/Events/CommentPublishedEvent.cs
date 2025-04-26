using Blog.Domain.Entities;
using Blog.Domain.Events.Base;

namespace Blog.Domain.Events;
public class CommentPublishedEvent : IDomainEvent
{
    public required Comment.EntityId CommentId { get; set; }
}

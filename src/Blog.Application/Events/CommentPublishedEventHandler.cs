using Blog.Application.Jobs;
using Blog.Application.Services.Jobs;
using Blog.Domain.Entities;
using Blog.Domain.Entities.Base;
using Blog.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Events;
public class CommentPublishedEventHandler : INotificationHandler<CommentPublishedEvent>
{
    private readonly IContext _context;
    private readonly IBackgroundJobProcessor _backgroundJobProcessor;

    public CommentPublishedEventHandler(IContext context, IBackgroundJobProcessor backgroundJobProcessor)
    {
        _context = context;
        _backgroundJobProcessor = backgroundJobProcessor;
    }

    public async Task Handle(CommentPublishedEvent notification, CancellationToken cancellationToken)
    {
        var comment = await _context.Get<Comment>()
            .Select(x => new
            {
                x.Id,
                x.ArticleId,
                Article = x.Article.Title,
                CommentAuthor = x.Author.UserName,
                ParentCommentAuthorEmail = x.ParentComment.Author.Email,
                ArticleAuthorEmail = x.Article.Author.Email,
            })
            .FirstAsync(x => x.Id == notification.CommentId, cancellationToken);

        //Move to appsetttings
        var articleLink = $"https://adamscrypt.com/articles/{comment.ArticleId.Value}";

        if (comment.ParentCommentAuthorEmail is not null)
        {
            _backgroundJobProcessor.Enqueue<IEmailJob, EmailJobParams>(new EmailJobParams
            {
                Email = comment.ParentCommentAuthorEmail,
                Subject = $"New comment on your reply for article '{comment.Article}'",
                Body = $"A new comment has been published on your reply by {comment.CommentAuthor}.\n" +
                    $"Link to article: {articleLink}"
            });
        }
        else
        {
            _backgroundJobProcessor.Enqueue<IEmailJob, EmailJobParams>(new EmailJobParams
            {
                Email = comment.ArticleAuthorEmail,
                Subject = $"New comment on your article '{comment.Article}'",
                Body = $"A new comment has been published on your article by {comment.CommentAuthor}.\n" +
                    $"Link to article: {articleLink}"
            });
        }
    }
}
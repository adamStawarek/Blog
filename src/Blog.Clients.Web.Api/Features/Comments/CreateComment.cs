using Blog.Application.Services.ApplicationUser;
using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Blog.Domain.Entities.Enumerators;
using Blog.Domain.Events;
using Blog.Infrastructure.Database;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using static Blog.Clients.Web.Api.Features.Comments.CreateComment;

namespace Blog.Clients.Web.Api.Features.Comments;
public static class CreateComment
{
    public sealed class Command : IRequest<Result<Comment.EntityId>>
    {
        public string Content { get; set; } = string.Empty;
        public Guid ArticleId { get; set; }
        public Guid? ParentId { get; set; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Content)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(c => c.ArticleId)
              .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Comment.EntityId>>
    {
        private readonly BlogDbContext _context;
        private readonly IApplicationUserProvider _userProvider;

        public Handler(BlogDbContext context, IApplicationUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public async Task<Result<Comment.EntityId>> Handle(Command request, CancellationToken cancellationToken)
        {
            var currentUser = await _userProvider.GetAsync(cancellationToken);

            if (!IsRequestValid(request, out var error))
            {
                return Result.Fail(error);
            }

            var comment = new Comment
            {
                Content = request.Content,
                ArticleId = new Article.EntityId(request.ArticleId),
                ParentCommentId = request.ParentId is not null ?
                    new Comment.EntityId(request.ParentId.Value) :
                    null,
                AuthorId = currentUser.Id
            };

            await _context.Comment.AddAsync(comment, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok(comment.Id);
        }

        private bool IsRequestValid(Command request, out string? error)
        {
            error = null;

            if (request.ParentId is not null)
            {
                var parentComment = _context.Comment
                    .Select(x => new { x.Id, x.ParentCommentId })
                    .First(c => c.Id == request.ParentId);

                if (parentComment.ParentCommentId is not null)
                {
                    error = "Comment nesting is too deep.";
                    return false;
                }
            }

            var articleStatus = _context.Article
                .Where(x => x.Id == request.ArticleId)
                .Select(x => x.Status)
                .First();

            if (articleStatus is not ArticleStatus.Ready)
            {
                error = "Cannot comment on unpublished articles.";
                return false;
            }

            return true;
        }
    }
}

public class CreateCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapPost("api/articles/{articleId}/comments", async (Guid articleId, CreateCommentRequest request, IMediator mediator) =>
        {
            var command = request.Adapt<Command>();
            command.ArticleId = articleId;

            var result = await mediator.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            await mediator.Publish(new CommentPublishedEvent
            {
                CommentId = result.Value
            });

            return Results.Ok(result.Value.Value);
        })
        .RequireAuthorization()
        .WithTags("Comments")
        .WithName("CreateComment")
        .Produces<Guid>();
}
using Blog.Application.Services.ApplicationUser;
using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Blog.Domain.Events;
using Blog.Infrastructure.Database;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            if (request.ParentId is not null)
            {
                var parentComment = await _context.Comment
                    .Select(x => new { x.Id, x.ParentCommentId })
                    .FirstAsync(c => c.Id == request.ParentId, cancellationToken);

                if (parentComment.ParentCommentId is not null)
                {
                    return Result.Fail("Comment nesting is too deep.");
                }
            }

            var comment = new Comment
            {
                Content = request.Content,
                ArticleId = new Domain.Entities.Base.Entity<Article, Guid>.EntityId(request.ArticleId),
                ParentCommentId = request.ParentId is not null ?
                    new Domain.Entities.Base.Entity<Comment, Guid>.EntityId(request.ParentId.Value) : null,
                AuthorId = currentUser.Id
            };

            await _context.Comment.AddAsync(comment, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok(comment.Id);
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
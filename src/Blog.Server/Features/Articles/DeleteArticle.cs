using Blog.Infrastructure.Database;
using Blog.Server.Auth;
using Carter;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Blog.Server.Features.Articles.DeleteArticle;

namespace Blog.Server.Features.Articles;
public static class DeleteArticle
{
    public sealed class Command : IRequest<Result>
    {
        public Guid ArticleId { get; set; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ArticleId)
                .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly BlogDbContext _context;

        public Handler(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var article = await _context.Article
                .FirstAsync(x => x.Id == request.ArticleId, cancellationToken);

            _context.Article.Remove(article);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

public class DeleteArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapDelete("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new Command
            {
                ArticleId = id
            });

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        })
        .RequireAuthorization(BlogAuthPolicies.AdminAccess)
        .WithTags("Articles")
        .WithName("DeleteArticle");
}
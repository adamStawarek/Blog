using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Blog.Server.Contracts;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Blog.Server.Features.Articles.EditArticle;

namespace Blog.Server.Features.Articles;
public static class EditArticle
{
    public sealed class Command : IRequest<Result>
    {
        public Guid ArticleId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public List<string> Tags { get; set; } = default!;
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(c => c.Content)
                .NotEmpty()
                .MaximumLength(4000);

            RuleFor(c => c.Tags)
                .Must(x => x.Any())
                .WithMessage("At least one tag must be provided");
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
            var article = await _context.Set<Article>()
                .FirstAsync(x => x.Id == request.ArticleId, cancellationToken);

            article.Title = request.Title;
            article.Content = request.Title;
            article.Tags = request.Tags;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

public class EditArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/articles/{id}", async (Guid id, EditArticleRequest request, ISender sender) =>
        {
            var command = request.Adapt<Command>();

            command.ArticleId = id;

            var result = await sender.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        });
    }
}
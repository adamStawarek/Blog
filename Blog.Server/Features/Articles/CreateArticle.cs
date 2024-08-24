using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;

namespace Blog.Server.Features.Articles;
public static class CreateArticle
{
    public sealed class Command : IRequest<Result<Article.EntityId>>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
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

    public sealed class Handler : IRequestHandler<Command, Result<Article.EntityId>>
    {
        private readonly BlogDbContext _context;

        public Handler(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Article.EntityId>> Handle(Command request, CancellationToken cancellationToken)
        {
            var article = new Article
            {
                Title = request.Title,
                Content = request.Content,
                Tags = request.Tags,
                //AuthorId = 
            };

            await _context.Set<Article>().AddAsync(article, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok(article.Id);
        }
    }

    public sealed record Request
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("api/articles", async (Request request, ISender sender) =>
        {
            var command = request.Adapt<Command>();

            var result = await sender.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            return Results.Ok(result.Value);
        });
    }

}
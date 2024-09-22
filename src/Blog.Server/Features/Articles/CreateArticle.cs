using Blog.Application.Services.ApplicationUser;
using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Blog.Server.Contracts;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using static Blog.Server.Features.Articles.CreateArticle;

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
        private readonly IApplicationUserProvider _userProvider;

        public Handler(BlogDbContext context, IApplicationUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public async Task<Result<Article.EntityId>> Handle(Command request, CancellationToken cancellationToken)
        {
            var currentUser = await _userProvider.GetAsync(cancellationToken);

            var article = new Article
            {
                Title = request.Title,
                Content = request.Content,
                Tags = request.Tags,
                AuthorId = currentUser.Id
            };

            await _context.Set<Article>().AddAsync(article, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok(article.Id);
        }
    }
}

public class CreateArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapPost("api/articles", async (CreateArticleRequest request, ISender sender) =>
        {
            var command = request.Adapt<Command>();

            var result = await sender.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            return Results.Ok(result.Value.Value);
        })
        .WithTags("Articles")
        .WithName("CreateArticle")
        .Produces<Guid>();
}
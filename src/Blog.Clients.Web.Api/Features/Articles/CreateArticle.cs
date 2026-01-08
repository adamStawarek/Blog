using Blog.Application.Jobs;
using Blog.Application.Services.ApplicationUser;
using Blog.Application.Services.Jobs;
using Blog.Clients.Web.Api.Auth;
using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Blog.Domain.Entities.Enumerators;
using Blog.Infrastructure.Database;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using static Blog.Clients.Web.Api.Features.Articles.CreateArticle;

namespace Blog.Clients.Web.Api.Features.Articles;

public static class CreateArticle
{
    public sealed class Command : IRequest<Result<Article.EntityId>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ArticleStatus Status { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(c => c.Description)
               .NotEmpty()
               .MaximumLength(400);

            RuleFor(c => c.Content)
                .NotEmpty()
                .MaximumLength(20_000);

            RuleFor(c => c.Status)
                .IsInEnum();

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
                Description = request.Description,
                Content = request.Content,
                Status = request.Status,
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
        .MapPost("api/articles", async (CreateArticleRequest request, ISender sender, IBackgroundJobProcessor jobProcessor) =>
        {
            var command = request.Adapt<Command>();

            var result = await sender.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            jobProcessor.Enqueue<IDatabaseBackupJob>();

            return Results.Ok(result.Value.Value);
        })
        .RequireAuthorization(BlogAuthPolicies.AdminAccess)
        .WithTags("Articles")
        .WithName("CreateArticle")
        .Produces<Guid>();
}
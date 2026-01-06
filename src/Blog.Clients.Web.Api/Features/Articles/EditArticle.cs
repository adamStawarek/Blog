using Blog.Application.Jobs;
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
using Microsoft.EntityFrameworkCore;
using static Blog.Clients.Web.Api.Features.Articles.EditArticle;

namespace Blog.Clients.Web.Api.Features.Articles;
public static class EditArticle
{
    public sealed class Command : IRequest<Result>
    {
        public Guid ArticleId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Content { get; set; } = default!;
        public ArticleStatus Status { get; set; }
        public List<string> Tags { get; set; } = default!;
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(c => c.Title)
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
            article.Description = request.Description;
            article.Content = request.Content;
            article.Tags = request.Tags;
            article.Status = request.Status;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

public class EditArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapPut("api/articles/{id}", async (Guid id, EditArticleRequest request, ISender sender, IBackgroundJobProcessor jobProcessor) =>
        {
            var command = request.Adapt<Command>();

            command.ArticleId = id;

            var result = await sender.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            jobProcessor.Enqueue<IDatabaseBackupJob>();

            return Results.Ok();
        })
        .RequireAuthorization(BlogAuthPolicies.AdminAccess)
        .WithTags("Articles")
        .WithName("EditArticle");
}
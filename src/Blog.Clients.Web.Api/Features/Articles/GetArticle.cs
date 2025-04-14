using Blog.Infrastructure.Database;
using Blog.Server.Contracts;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Blog.Server.Features.Articles.GetArticle;

namespace Blog.Server.Features.Articles;
public static class GetArticle
{
    public sealed class Query : IRequest<Result<Response>>
    {
        public Guid ArticleId { get; set; }
    }

    internal sealed class Response
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<string> Tags { get; set; } = null!;
        public List<Comment> Comments { get; set; } = null!;

        internal sealed class Comment
        {
            public Guid Id { get; set; }
            public string Content { get; set; } = null!;
            public string Author { get; set; } = null!;
            public DateTime Date { get; set; }
            public List<Comment> Replies { get; set; } = null!;
        }
    }

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ArticleId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly BlogDbContext _context;

        public Handler(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var article = await _context.Article
                .AsNoTracking()
                .Select(x => new Response
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Content = x.Content,
                    Tags = x.Tags,
                    Author = x.Author.UserName!,
                    Date = x.Meta_CreatedDate.Date,
                    Comments = x.Comments
                        .Where(c => c.ParentCommentId == null)
                        .Select(c => new Response.Comment
                        {
                            Id = c.Id,
                            Content = c.Content,
                            Author = c.Author.UserName!,
                            Date = c.Meta_CreatedDate.Date,
                            Replies = c.ChildComments
                                .Select(r => new Response.Comment
                                {
                                    Id = r.Id,
                                    Content = r.Content,
                                    Author = r.Author.UserName!,
                                    Date = r.Meta_CreatedDate.Date
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstAsync(x => x.Id == request.ArticleId, cancellationToken);

            return Result.Ok(article);
        }
    }
}

public class GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapGet("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new Query
            {
                ArticleId = id
            });

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            var response = result.Value.Adapt<GetArticleResponse>();

            return Results.Ok(response);
        })
        .WithTags("Articles")
        .WithName("GetArticle")
        .Produces<GetArticleResponse>();
}
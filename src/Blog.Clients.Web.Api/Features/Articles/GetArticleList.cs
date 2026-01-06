using Blog.Application.Services.ApplicationUser;
using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Blog.Domain.Entities.Enumerators;
using Blog.Domain.Roles;
using Blog.Infrastructure.Database;
using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Blog.Clients.Web.Api.Features.Articles.GetArticleList;

namespace Blog.Clients.Web.Api.Features.Articles;
public static class GetArticleList
{
    public sealed class Query : IRequest<Result<Response>>
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }

    internal sealed class Response
    {
        public long TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public ICollection<ArticleItem> Items { get; set; } = null!;

        public sealed class ArticleItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = null!;
            public string Author { get; set; } = null!;
            public string Description { get; set; } = null!;
            public ArticleStatus Status { get; set; }
            public List<string> Tags { get; set; } = null!;
            public DateTime Date { get; set; }
        }
    }

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ItemsPerPage).InclusiveBetween(1, 20);
            RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly BlogDbContext _context;
        private readonly IApplicationUserProvider _userProvider;

        public Handler(BlogDbContext context, IApplicationUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUser = await _userProvider.GetAsync(cancellationToken);

            Expression<Func<Article, bool>> statusFilter = currentUser.Role is ApplicationRole.Administrator ?
                _ => true :
                x => x.Status == ArticleStatus.Ready;

            var total = await _context.Article
                .Where(statusFilter)
                .CountAsync(cancellationToken);

            var articles = await _context.Article
                .AsNoTracking()
                .OrderByDescending(x => x.Meta_CreatedDate)
                .Where(statusFilter)
                .Skip(request.Page * request.ItemsPerPage)
                .Take(request.ItemsPerPage)
                .Select(x => new Response.ArticleItem
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Author = x.Author.UserName!,
                    Status = x.Status,
                    Tags = x.Tags,
                    Date = x.Meta_CreatedDate.Date
                })
                .ToListAsync(cancellationToken);

            return Result.Ok(new Response
            {
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)request.ItemsPerPage),
                CurrentPage = request.Page,
                Items = articles
            });
        }
    }
}

public class GetArticlesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapGet("api/articles", async ([AsParameters] GetArticleListRequest request, ISender sender) =>
        {
            var command = request.Adapt<Query>();

            var result = await sender.Send(command);

            if (result.IsFailed)
            {
                return Results.BadRequest();
            }

            var response = result.Value.Adapt<GetArticleListResponse>();

            return Results.Ok(response);
        })
        .WithTags("Articles")
        .WithName("GetArticles")
        .Produces<GetArticleListResponse>();
}
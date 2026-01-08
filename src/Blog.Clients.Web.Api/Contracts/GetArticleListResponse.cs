using Blog.Clients.Web.Api.Contracts.Base;
using Blog.Domain.Entities.Enumerators;

namespace Blog.Clients.Web.Api.Contracts;

public class GetArticleListResponse : PageableResponse<GetArticleListResponse.ArticleItem>
{
    public sealed record ArticleItem
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }
        public ArticleStatus Status { get; set; }
        public required List<string> Tags { get; set; }
        public required DateTime Date { get; set; }
    }
}
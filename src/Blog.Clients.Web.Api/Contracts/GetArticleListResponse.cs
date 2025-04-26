using Blog.Clients.Web.Api.Contracts.Base;

namespace Blog.Clients.Web.Api.Contracts;
public class GetArticleListResponse : PageableResponse<GetArticleListResponse.ArticleItem>
{
    public sealed record ArticleItem
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }
        public required List<string> Tags { get; set; }
        public required DateTime Date { get; set; }
    }
}
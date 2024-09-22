using Blog.Server.Contracts.Base;

namespace Blog.Server.Contracts;
public class GetArticleListResponse : PageableResponse<GetArticleListResponse.ArticleItem>
{
    public sealed record ArticleItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
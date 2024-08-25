namespace Blog.Server.Contracts;
public class GetArticleResponse
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Date { get; set; }
}
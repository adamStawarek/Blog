namespace Blog.Server.Contracts;
public class EditArticleRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Content { get; set; }
    public required List<string> Tags { get; set; }
}
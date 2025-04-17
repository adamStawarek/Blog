namespace Blog.Server.Contracts;
public class GetArticleResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Author { get; set; }
    public required string Content { get; set; }
    public required List<string> Tags { get; set; }
    public required List<Comment> Comments { get; set; }
    public required DateTime Date { get; set; }

    public class Comment
    {
        public required Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Author { get; set; }
        public required DateTime Date { get; set; }
        public required List<Comment> Replies { get; set; }
    }
}
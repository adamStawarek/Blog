namespace Blog.Clients.Web.Api.Contracts;
public class CreateCommentRequest
{
    public required string Content { get; set; }
    public Guid? ParentId { get; set; }
}
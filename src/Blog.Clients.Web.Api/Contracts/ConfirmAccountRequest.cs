namespace Blog.Clients.Web.Api.Contracts;

public class ConfirmAccountRequest
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}
namespace Blog.Clients.Web.Api.Contracts;
public class GetAccountInfoResponse
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required List<string> Roles { get; set; }
}
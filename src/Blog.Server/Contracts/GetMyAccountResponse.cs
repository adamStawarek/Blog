namespace Blog.Server.Contracts;
public class GetMyAccountResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public List<string> Roles { get; set; } = null!;
}
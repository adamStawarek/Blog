namespace Blog.Clients.Web.Api.Contracts.Base;
public abstract class PageableRequest
{
    public required int Page { get; set; }
    public required int ItemsPerPage { get; set; }
}
namespace Blog.Clients.Web.Api.Contracts.Base;
public class PageableResponse<T>
{
    public required long TotalItems { get; set; }
    public required int TotalPages { get; set; }
    public required int CurrentPage { get; set; }
    public required List<T> Items { get; set; }
}
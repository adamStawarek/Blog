namespace Blog.Server.Contracts.Base;
public class PageableResponse<T>
{
    public long TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<T> Items { get; set; } = null!;
}
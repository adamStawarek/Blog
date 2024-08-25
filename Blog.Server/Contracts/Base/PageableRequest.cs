namespace Blog.Server.Contracts.Base;
public abstract class PageableRequest
{
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
}
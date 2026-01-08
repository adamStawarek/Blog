using Blog.Domain.Entities.Base;
using Blog.Domain.Entities.Enumerators;

namespace Blog.Domain.Entities;

public class Article : EntityBase<Article>
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ArticleStatus Status { get; set; }
    public List<string> Tags { get; set; } = null!;

    #region Relations

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = null!;

    #endregion
}
using Blog.Domain.Entities.Base;

namespace Blog.Domain.Entities;
public class Comment : EntityBase<Comment>
{
    public string Content { get; set; } = null!;

    #region Relations

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public Article.EntityId ArticleId { get; set; } = null!;
    public Article Article { get; set; } = null!;

    #endregion
}
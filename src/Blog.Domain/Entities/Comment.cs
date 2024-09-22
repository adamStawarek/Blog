using Blog.Domain.Entities.Base;

namespace Blog.Domain.Entities;
public class Comment : EntityBase<Comment>
{
    public string Content { get; set; } = null!;

    #region Relations

    public User.EntityId AuthorId { get; set; } = null!;
    public User Author { get; set; } = null!;

    public Article.EntityId ArticleId { get; set; } = null!;
    public Article Article { get; set; } = null!;

    #endregion
}
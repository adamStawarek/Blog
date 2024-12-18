using Blog.Domain.Entities.Base;

namespace Blog.Domain.Entities;
public class Article : EntityBase<Article>
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;

    #region Relations

    public User.EntityId AuthorId { get; set; } = null!;
    public User Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = null!;

    #endregion
}